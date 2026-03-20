import { apiRequest } from "./httpClient";

export interface DashboardSummary {
  totalIncome: number;
  totalExpenses: number;
  netAmount: number;
  budgetUsage: Array<{
    categoryName: string;
    budgetAmount: number;
    spent: number;
    remaining: number;
    percentageUsed: number;
  }>;
  upcomingBills: Array<{
    name: string;
    amount: number;
    dueDate: string;
    daysUntilDue: number;
    isPaid: boolean;
  }>;
  recentTransactions: Array<{
    id: number;
    description: string;
    amount: number;
    date: string;
    type: string;
    category: string;
  }>;
  savingsProgress: Array<{
    goalName: string;
    currentAmount: number;
    targetAmount: number;
    percentageComplete: number;
    remaining: number;
  }>;
}

export interface CategoryDto {
  id: number;
  name: string;
  type: "Expense" | "Income";
  isSystemDefault: boolean;
  color?: string | null;
}

export interface ExpenseDto {
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

export interface IncomeDto {
  id: number;
  amount: number;
  source: string;
  date: string;
  categoryId: number;
  categoryName: string;
  userId: number;
  userName: string;
}

export interface BillDto {
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

export interface SavingsGoalDto {
  id: number;
  name: string;
  targetAmount: number;
  currentAmount: number;
  targetDate?: string | null;
  priority: string;
  percentageComplete: number;
  remaining: number;
  status: string;
  completedDate?: string | null;
  isCompleted: boolean;
}

export interface BudgetDto {
  id: number;
  amount: number;
  month: string;
  categoryId: number;
  categoryName: string;
  currentSpent: number;
  remaining: number;
  percentageUsed: number;
}

export interface HouseholdMemberDto {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
}

export interface HouseholdDto {
  id: number;
  name: string;
  currencySymbol: string;
  members: HouseholdMemberDto[];
}

export const financeApi = {
  getDashboardSummary: (token: string, year: number, month: number) =>
    apiRequest<DashboardSummary>(`/dashboard/summary?year=${year}&month=${month}`, {
      method: "GET",
      token,
    }),

  getCategories: (token: string, type?: "Expense" | "Income") => {
    const query = type ? `?type=${encodeURIComponent(type)}` : "";
    return apiRequest<CategoryDto[]>(`/categories${query}`, { method: "GET", token });
  },

  createCategory: (token: string, payload: { name: string; type: "Expense" | "Income"; color?: string }) =>
    apiRequest<CategoryDto>("/categories", {
      method: "POST",
      token,
      body: JSON.stringify(payload),
    }),

  updateCategory: (token: string, categoryId: number, payload: { name?: string; color?: string }) =>
    apiRequest<CategoryDto>(`/categories/${categoryId}`, {
      method: "PUT",
      token,
      body: JSON.stringify(payload),
    }),

  deleteCategory: (token: string, categoryId: number) =>
    apiRequest<void>(`/categories/${categoryId}`, {
      method: "DELETE",
      token,
    }),

  getExpenses: (token: string) =>
    apiRequest<ExpenseDto[]>("/expenses?pageNumber=1&pageSize=100", { method: "GET", token }),

  createExpense: (
    token: string,
    payload: { amount: number; description: string; isShared: boolean; date: string; categoryId: number },
  ) =>
    apiRequest<ExpenseDto>("/expenses", {
      method: "POST",
      token,
      body: JSON.stringify(payload),
    }),

  getIncome: (token: string) => apiRequest<IncomeDto[]>("/income", { method: "GET", token }),

  createIncome: (
    token: string,
    payload: { amount: number; source: string; date: string; categoryId: number },
  ) =>
    apiRequest<IncomeDto>("/income", {
      method: "POST",
      token,
      body: JSON.stringify(payload),
    }),

  getBills: (token: string) => apiRequest<BillDto[]>("/bills", { method: "GET", token }),

  createBill: (
    token: string,
    payload: { name: string; amount: number; dueDate: string; frequency: string; categoryId: number },
  ) =>
    apiRequest<BillDto>("/bills", {
      method: "POST",
      token,
      body: JSON.stringify(payload),
    }),

  markBillAsPaid: (token: string, billId: number, paidDate: string) =>
    apiRequest<BillDto>(`/bills/${billId}/pay`, {
      method: "POST",
      token,
      body: JSON.stringify({ paidDate }),
    }),

  getSavingsGoals: (token: string) =>
    apiRequest<SavingsGoalDto[]>("/savings-goals", { method: "GET", token }),

  createSavingsGoal: (
    token: string,
    payload: { name: string; targetAmount: number; targetDate?: string; priority: string },
  ) =>
    apiRequest<SavingsGoalDto>("/savings-goals", {
      method: "POST",
      token,
      body: JSON.stringify(payload),
    }),

  updateSavingsGoal: (
    token: string,
    goalId: number,
    payload: { name?: string; targetAmount?: number; targetDate?: string | null; priority?: string; status?: string },
  ) =>
    apiRequest<SavingsGoalDto>(`/savings-goals/${goalId}`, {
      method: "PUT",
      token,
      body: JSON.stringify(payload),
    }),

  deleteSavingsGoal: (token: string, goalId: number) =>
    apiRequest<void>(`/savings-goals/${goalId}`, { method: "DELETE", token }),

  createGoalContribution: (token: string, goalId: number, payload: { amount: number; contributionDate: string }) =>
    apiRequest(`/goals/${goalId}/contributions`, {
      method: "POST",
      token,
      body: JSON.stringify({ ...payload, goalId }),
    }),

  getBudgets: (token: string, year: number, month: number) =>
    apiRequest<BudgetDto[]>(`/budgets?year=${year}&month=${month}`, { method: "GET", token }),

  createBudget: (token: string, payload: { amount: number; month: string; categoryId: number }) =>
    apiRequest<BudgetDto>("/budgets", {
      method: "POST",
      token,
      body: JSON.stringify(payload),
    }),

  updateBudget: (token: string, budgetId: number, payload: { amount: number }) =>
    apiRequest<BudgetDto>(`/budgets/${budgetId}`, {
      method: "PUT",
      token,
      body: JSON.stringify(payload),
    }),

  deleteBudget: (token: string, budgetId: number) =>
    apiRequest<void>(`/budgets/${budgetId}`, {
      method: "DELETE",
      token,
    }),

  getHousehold: (token: string) => apiRequest<HouseholdDto>("/households", { method: "GET", token }),

  getHouseholdMembers: (token: string) =>
    apiRequest<HouseholdMemberDto[]>("/households/members", { method: "GET", token }),
};
