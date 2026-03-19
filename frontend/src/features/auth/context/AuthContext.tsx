import { createContext, useContext, useEffect, useMemo, useState, type ReactNode } from "react";
import { authApi } from "../api/authApi";
import type { CurrentUser, LoginRequest, RegisterRequest } from "../../../shared/types/api";

const TOKEN_KEY = "hb_auth_token";

interface AuthContextValue {
  user: CurrentUser | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (payload: LoginRequest) => Promise<void>;
  register: (payload: RegisterRequest) => Promise<void>;
  logout: () => void;
  refreshCurrentUser: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

interface AuthProviderProps {
  children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [token, setToken] = useState<string | null>(() => localStorage.getItem(TOKEN_KEY));
  const [user, setUser] = useState<CurrentUser | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  const persistToken = (nextToken: string | null) => {
    if (nextToken) {
      localStorage.setItem(TOKEN_KEY, nextToken);
    } else {
      localStorage.removeItem(TOKEN_KEY);
    }
    setToken(nextToken);
  };

  const refreshCurrentUser = async () => {
    if (!token) {
      setUser(null);
      return;
    }

    try {
      const currentUser = await authApi.me(token);
      setUser(currentUser);
    } catch {
      setUser(null);
      persistToken(null);
    }
  };

  const login = async (payload: LoginRequest) => {
    const response = await authApi.login(payload);
    persistToken(response.token);
    setUser(response.user);
  };

  const register = async (payload: RegisterRequest) => {
    const response = await authApi.register(payload);
    persistToken(response.token);
    setUser(response.user);
  };

  const logout = () => {
    setUser(null);
    persistToken(null);
  };

  useEffect(() => {
    let mounted = true;

    const bootstrap = async () => {
      if (token) {
        try {
          const currentUser = await authApi.me(token);
          if (mounted) setUser(currentUser);
        } catch {
          if (mounted) {
            setUser(null);
            persistToken(null);
          }
        }
      }
      if (mounted) setIsLoading(false);
    };

    bootstrap();

    return () => {
      mounted = false;
    };
  }, [token]);

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      token,
      isAuthenticated: Boolean(token && user),
      isLoading,
      login,
      register,
      logout,
      refreshCurrentUser,
    }),
    [user, token, isLoading],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used inside AuthProvider");
  }
  return context;
}
