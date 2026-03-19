import { apiRequest } from "../../../shared/api/httpClient";
import type { Bill, Budget, Category, Expense, Income, SavingsGoal } from "../../../shared/types/api";

export const financeApi = {
  getCategories: (token: string, type?: "Expense" | "Income") =>
    apiRequest<Category[]>(`/categories${type ? `?type=${type}` : ""}`, {
      method: "GET",
      token,
    }),

  getExpenses: (token: string) =>
    apiRequest<Expense[]>("/expenses", {
      method: "GET",
      token,
    }),

  getIncome: (token: string) =>
    apiRequest<Income[]>("/income", {
      method: "GET",
      token,
    }),

  getBudgets: (token: string, year: number, month: number) =>
    apiRequest<Budget[]>(`/budgets?year=${year}&month=${month}`, {
      method: "GET",
      token,
    }),

  getBills: (token: string) =>
    apiRequest<Bill[]>("/bills", {
      method: "GET",
      token,
    }),

  getUpcomingBills: (token: string) =>
    apiRequest<Bill[]>("/bills/upcoming", {
      method: "GET",
      token,
    }),

  getSavingsGoals: (token: string) =>
    apiRequest<SavingsGoal[]>("/savings-goals", {
      method: "GET",
      token,
    }),
};
