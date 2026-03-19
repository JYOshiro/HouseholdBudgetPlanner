import { useEffect, useMemo, useState } from "react";
import { User, Bell, Shield, Palette, Users, Mail, Moon, Sun } from "lucide-react";
import { useAuth } from "../../auth/context/AuthContext";
import { useTheme } from "../../../app/context/ThemeContext";
import { ApiError } from "../../../shared/api/httpClient";
import { financeApi, type HouseholdMemberDto } from "../../../shared/api/financeApi";

type Preferences = {
  currency: string;
  dateFormat: string;
  firstDayOfWeek: string;
  billReminders: boolean;
  budgetAlerts: boolean;
  savingsMilestones: boolean;
  emailSummaries: boolean;
};

const PREFERENCES_KEY = "hb_user_preferences";

const defaultPreferences: Preferences = {
  currency: "USD",
  dateFormat: "MM/DD/YYYY",
  firstDayOfWeek: "Sunday",
  billReminders: true,
  budgetAlerts: true,
  savingsMilestones: true,
  emailSummaries: false,
};

export default function SettingsPage() {
  const { user, token, logout } = useAuth();
  const { theme, toggleTheme } = useTheme();
  const [members, setMembers] = useState<HouseholdMemberDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [notice, setNotice] = useState<string | null>(null);

  const [preferences, setPreferences] = useState<Preferences>(() => {
    const saved = localStorage.getItem(PREFERENCES_KEY);
    if (!saved) return defaultPreferences;
    try {
      return { ...defaultPreferences, ...(JSON.parse(saved) as Partial<Preferences>) };
    } catch {
      return defaultPreferences;
    }
  });

  useEffect(() => {
    const loadMembers = async () => {
      if (!token) return;
      setLoading(true);
      setError(null);
      try {
        const data = await financeApi.getHouseholdMembers(token);
        setMembers(data);
      } catch (err) {
        setError(err instanceof ApiError ? err.message : "Failed to load household members.");
      } finally {
        setLoading(false);
      }
    };

    void loadMembers();
  }, [token]);

  const initials = useMemo(() => {
    const first = user?.firstName?.[0] ?? "U";
    const last = user?.lastName?.[0] ?? "";
    return `${first}${last}`.toUpperCase();
  }, [user]);

  const savePreferences = () => {
    localStorage.setItem(PREFERENCES_KEY, JSON.stringify(preferences));
    setNotice("Preferences saved locally.");
  };

  const handleToggle = (key: keyof Preferences) => {
    setPreferences((prev) => ({ ...prev, [key]: !prev[key] }));
  };

  return (
    <div className="max-w-4xl mx-auto space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Settings</h1>
        <p className="text-gray-600 mt-1">Manage your account view and preferences</p>
      </div>

      {error && <div className="p-3 rounded-xl bg-red-50 border border-red-200 text-red-700">{error}</div>}
      {notice && <div className="p-3 rounded-xl bg-green-50 border border-green-200 text-green-700">{notice}</div>}

      <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
        <div className="flex items-center gap-3 mb-6">
          <div className="w-10 h-10 bg-blue-100 rounded-xl flex items-center justify-center">
            <User className="w-5 h-5 text-blue-600" />
          </div>
          <h2 className="text-lg font-semibold text-gray-900">Profile</h2>
        </div>

        <div className="space-y-4">
          <div className="flex items-center gap-4">
            <div className="w-20 h-20 bg-gradient-to-br from-blue-400 to-purple-500 rounded-full flex items-center justify-center text-white text-2xl font-bold">
              {initials}
            </div>
            <div>
              <p className="font-medium text-gray-900">{user ? `${user.firstName} ${user.lastName}` : "User"}</p>
              <p className="text-sm text-gray-600">{user?.email ?? ""}</p>
              <p className="text-xs text-gray-500 mt-1">Profile editing endpoint is not implemented on backend yet.</p>
            </div>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <ReadOnlyField label="First Name" value={user?.firstName ?? ""} />
            <ReadOnlyField label="Last Name" value={user?.lastName ?? ""} />
          </div>

          <ReadOnlyField label="Email" value={user?.email ?? ""} />
        </div>
      </div>

      <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
        <div className="flex items-center gap-3 mb-6">
          <div className="w-10 h-10 bg-purple-100 rounded-xl flex items-center justify-center">
            <Users className="w-5 h-5 text-purple-600" />
          </div>
          <h2 className="text-lg font-semibold text-gray-900">Household Members</h2>
        </div>

        {loading ? (
          <p className="text-gray-500">Loading members...</p>
        ) : (
          <div className="space-y-3 mb-4">
            {members.map((member) => {
              const memberInitials = `${member.firstName[0] ?? ""}${member.lastName[0] ?? ""}`.toUpperCase();
              const isCurrentUser = member.id === user?.id;
              return (
                <div key={member.id} className="flex items-center justify-between p-4 bg-gray-50 rounded-xl">
                  <div className="flex items-center gap-3">
                    <div className="w-12 h-12 bg-gradient-to-br from-blue-400 to-purple-500 rounded-full flex items-center justify-center text-white font-bold">
                      {memberInitials}
                    </div>
                    <div>
                      <p className="font-medium text-gray-900">{member.fullName}</p>
                      <p className="text-sm text-gray-600">{member.email}</p>
                    </div>
                  </div>
                  <span className={`px-3 py-1 rounded-full text-xs font-medium ${isCurrentUser ? "bg-blue-100 text-blue-700" : "bg-gray-200 text-gray-700"}`}>
                    {isCurrentUser ? "You" : "Member"}
                  </span>
                </div>
              );
            })}
          </div>
        )}

        <button disabled className="w-full py-2.5 border-2 border-dashed border-gray-300 text-gray-400 rounded-xl font-medium cursor-not-allowed">
          Invite Household Member (coming soon)
        </button>
      </div>

      <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
        <div className="flex items-center gap-3 mb-6">
          <div className="w-10 h-10 bg-orange-100 rounded-xl flex items-center justify-center">
            <Bell className="w-5 h-5 text-orange-600" />
          </div>
          <h2 className="text-lg font-semibold text-gray-900">Notifications</h2>
        </div>

        <div className="space-y-4">
          <ToggleRow title="Bill Reminders" description="Get notified before bills are due" checked={preferences.billReminders} onToggle={() => handleToggle("billReminders")} />
          <ToggleRow title="Budget Alerts" description="Alert when reaching budget limits" checked={preferences.budgetAlerts} onToggle={() => handleToggle("budgetAlerts")} />
          <ToggleRow title="Savings Milestones" description="Celebrate savings goal milestones" checked={preferences.savingsMilestones} onToggle={() => handleToggle("savingsMilestones")} />
          <ToggleRow title="Email Summaries" description="Weekly finance summary email" checked={preferences.emailSummaries} onToggle={() => handleToggle("emailSummaries")} />
        </div>
      </div>

      <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
        <div className="flex items-center gap-3 mb-6">
          <div className="w-10 h-10 bg-green-100 rounded-xl flex items-center justify-center">
            <Palette className="w-5 h-5 text-green-600" />
          </div>
          <h2 className="text-lg font-semibold text-gray-900">Preferences</h2>
        </div>

        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Currency</label>
            <select value={preferences.currency} onChange={(e) => setPreferences((prev) => ({ ...prev, currency: e.target.value }))} className="w-full px-4 py-2 border border-gray-200 rounded-xl">
              <option value="USD">USD - US Dollar ($)</option>
              <option value="EUR">EUR - Euro (€)</option>
              <option value="GBP">GBP - British Pound (£)</option>
              <option value="CAD">CAD - Canadian Dollar (C$)</option>
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Date Format</label>
            <select value={preferences.dateFormat} onChange={(e) => setPreferences((prev) => ({ ...prev, dateFormat: e.target.value }))} className="w-full px-4 py-2 border border-gray-200 rounded-xl">
              <option value="MM/DD/YYYY">MM/DD/YYYY</option>
              <option value="DD/MM/YYYY">DD/MM/YYYY</option>
              <option value="YYYY-MM-DD">YYYY-MM-DD</option>
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">First Day of Week</label>
            <select value={preferences.firstDayOfWeek} onChange={(e) => setPreferences((prev) => ({ ...prev, firstDayOfWeek: e.target.value }))} className="w-full px-4 py-2 border border-gray-200 rounded-xl">
              <option value="Sunday">Sunday</option>
              <option value="Monday">Monday</option>
            </select>
          </div>

          <button onClick={savePreferences} className="px-6 py-2 bg-gradient-to-r from-blue-500 to-purple-600 text-white rounded-xl font-medium hover:shadow-lg">
            Save Preferences
          </button>
        </div>
      </div>

      <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
        <div className="flex items-center gap-3 mb-6">
          <div className="w-10 h-10 bg-indigo-100 rounded-xl flex items-center justify-center">
            {theme === "dark" ? <Moon className="w-5 h-5 text-indigo-600" /> : <Sun className="w-5 h-5 text-indigo-600" />}
          </div>
          <h2 className="text-lg font-semibold text-gray-900">Appearance</h2>
        </div>

        <button onClick={toggleTheme} className="px-4 py-2 bg-gray-100 text-gray-700 rounded-lg text-sm font-medium hover:bg-gray-200">
          Toggle Theme ({theme})
        </button>
      </div>

      <div className="bg-white rounded-2xl p-6 border border-red-200 shadow-sm">
        <div className="flex items-center gap-3 mb-4">
          <div className="w-10 h-10 bg-red-100 rounded-xl flex items-center justify-center">
            <Shield className="w-5 h-5 text-red-600" />
          </div>
          <h2 className="text-lg font-semibold text-red-900">Account Actions</h2>
        </div>

        <div className="space-y-3">
          <button disabled className="w-full px-4 py-2 bg-red-50 text-red-400 rounded-xl font-medium border border-red-200 cursor-not-allowed">
            Change Password (coming soon)
          </button>
          <button onClick={logout} className="w-full px-4 py-2 bg-gray-100 text-gray-700 rounded-xl font-medium hover:bg-gray-200">
            Log Out
          </button>
          <button disabled className="w-full px-4 py-2 bg-red-50 text-red-400 rounded-xl font-medium border border-red-200 cursor-not-allowed">
            Delete Account (coming soon)
          </button>
        </div>
      </div>
    </div>
  );
}

function ReadOnlyField({ label, value }: { label: string; value: string }) {
  return (
    <div>
      <label className="block text-sm font-medium text-gray-700 mb-2">{label}</label>
      <input type="text" value={value} disabled className="w-full px-4 py-2 border border-gray-200 rounded-xl bg-gray-50 text-gray-700" />
    </div>
  );
}

function ToggleRow({ title, description, checked, onToggle }: { title: string; description: string; checked: boolean; onToggle: () => void }) {
  return (
    <div className="flex items-center justify-between">
      <div>
        <p className="font-medium text-gray-900">{title}</p>
        <p className="text-sm text-gray-600">{description}</p>
      </div>
      <label className="relative inline-flex items-center cursor-pointer">
        <input type="checkbox" checked={checked} onChange={onToggle} className="sr-only peer" />
        <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-2 peer-focus:ring-blue-500 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600" />
      </label>
    </div>
  );
}
