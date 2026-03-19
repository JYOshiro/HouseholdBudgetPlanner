import { createBrowserRouter } from "react-router";
import AppLayout from "../../shared/layout/AppLayout";
import LandingPage from "../../features/landing/pages/LandingPage";
import DashboardPage from "../../features/dashboard/pages/DashboardPage";
import TransactionsPage from "../../features/transactions/pages/TransactionsPage";
import BudgetPage from "../../features/budget/pages/BudgetPage";
import BillsPage from "../../features/bills/pages/BillsPage";
import SavingsPage from "../../features/savings/pages/SavingsPage";
import HouseholdPage from "../../features/household/pages/HouseholdPage";
import SettingsPage from "../../features/settings/pages/SettingsPage";
import LoginPage from "../../features/auth/pages/LoginPage";
import RegisterPage from "../../features/auth/pages/RegisterPage";
import RequireAuth from "../../features/auth/components/RequireAuth";
import RedirectIfAuthenticated from "../../features/auth/components/RedirectIfAuthenticated";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <LandingPage />,
  },
  {
    path: "/login",
    element: (
      <RedirectIfAuthenticated>
        <LoginPage />
      </RedirectIfAuthenticated>
    ),
  },
  {
    path: "/register",
    element: (
      <RedirectIfAuthenticated>
        <RegisterPage />
      </RedirectIfAuthenticated>
    ),
  },
  {
    path: "/app",
    element: (
      <RequireAuth>
        <AppLayout />
      </RequireAuth>
    ),
    children: [
      { index: true, element: <DashboardPage /> },
      { path: "transactions", element: <TransactionsPage /> },
      { path: "budget", element: <BudgetPage /> },
      { path: "bills", element: <BillsPage /> },
      { path: "savings", element: <SavingsPage /> },
      { path: "household", element: <HouseholdPage /> },
      { path: "settings", element: <SettingsPage /> },
    ],
  },
]);
