export interface CurrentUser {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  householdId: number;
}

export interface AuthResponse {
  token: string;
  expiresIn: number;
  user: CurrentUser;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  householdName: string;
}

export interface Category {
  id: number;
  name: string;
  type: "Expense" | "Income";
  isSystemDefault: boolean;
  color: string;
}

export interface Expense {
  id: number;
  amount: number;
  description: string;
  isShared: boolean;
  date: string;
  categoryId: number;
  categoryName: string;
  paidByUserId: number;
  paidByUserName: string;
}

export interface Income {
  id: number;
  amount: number;
  source: string;
  date: string;
  categoryId: number;
  categoryName: string;
  userId: number;
  userName: string;
}

export interface Budget {
  id: number;
  amount: number;
  month: string;
  categoryId: number;
  categoryName: string;
  currentSpent: number;
  remaining: number;
  percentageUsed: number;
}

export interface Bill {
  id: number;
  name: string;
  amount: number;
  dueDate: string;
  frequency: string;
  isPaid: boolean;
  lastPaidDate?: string | null;
  categoryId: number;
  categoryName: string;
  daysUntilDue: number;
}

export interface SavingsGoal {
  id: number;
  name: string;
  targetAmount: number;
  currentAmount: number;
  targetDate: string;
  priority: string;
  percentageComplete: number;
  remaining: number;
}
