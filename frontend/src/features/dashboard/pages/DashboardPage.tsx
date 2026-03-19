import { useEffect, useMemo, useState } from "react";
import {
  TrendingUp,
  TrendingDown,
  DollarSign,
  Calendar,
  Target,
  ArrowUpRight,
  ArrowDownRight,
} from "lucide-react";
import { PieChart, Pie, Cell, ResponsiveContainer, Tooltip } from "recharts";
import { useAuth } from "../../auth/context/AuthContext";
import { ApiError } from "../../../shared/api/httpClient";
import { financeApi, type DashboardSummary } from "../../../shared/api/financeApi";

const chartColors = ["#3b82f6", "#8b5cf6", "#ec4899", "#f59e0b", "#10b981", "#ef4444", "#14b8a6"];

export default function DashboardPage() {
  const { token } = useAuth();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [summary, setSummary] = useState<DashboardSummary | null>(null);

  const now = useMemo(() => new Date(), []);
  const year = now.getFullYear();
  const month = now.getMonth() + 1;

  const loadSummary = async () => {
    if (!token) return;
    setLoading(true);
    setError(null);

    try {
      const data = await financeApi.getDashboardSummary(token, year, month);
      setSummary(data);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to load dashboard data.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadSummary();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token]);

  const budgetData = (summary?.budgetUsage ?? []).map((item, idx) => ({
    name: item.categoryName,
    value: Number(item.spent),
    color: chartColors[idx % chartColors.length],
  })).filter((item) => Number.isFinite(item.value) && item.value > 0);

  const totalBudget = (summary?.budgetUsage ?? []).reduce((sum, item) => sum + Number(item.budgetAmount), 0);
  const totalSpent = Number(summary?.totalExpenses ?? 0);
  const budgetProgress = totalBudget > 0 ? (totalSpent / totalBudget) * 100 : 0;

  const totalSavingsCurrent = (summary?.savingsProgress ?? []).reduce((sum, g) => sum + Number(g.currentAmount), 0);

  return (
    <div className="max-w-7xl mx-auto space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Dashboard</h1>
        <p className="text-gray-600 mt-1">Live household financial overview for {now.toLocaleString("en-US", { month: "long", year: "numeric" })}</p>
      </div>

      {error && <div className="p-3 rounded-xl bg-red-50 border border-red-200 text-red-700">{error}</div>}

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <TopCard icon={<DollarSign className="w-6 h-6 text-blue-600" />} title="Net Amount" value={`$${Number(summary?.netAmount ?? 0).toFixed(2)}`} trend={<TrendingUp className="w-4 h-4" />} />
        <TopCard icon={<TrendingUp className="w-6 h-6 text-green-600" />} title="Monthly Income" value={`$${Number(summary?.totalIncome ?? 0).toFixed(2)}`} trend={<span className="text-green-600 text-sm font-medium">Live</span>} />
        <TopCard icon={<TrendingDown className="w-6 h-6 text-orange-600" />} title="Monthly Expenses" value={`$${Number(summary?.totalExpenses ?? 0).toFixed(2)}`} trend={<span className="text-orange-600 text-sm font-medium">Live</span>} />
        <TopCard icon={<Target className="w-6 h-6 text-purple-600" />} title="Savings Progress" value={`$${totalSavingsCurrent.toFixed(2)}`} trend={<span className="text-purple-600 text-sm font-medium">{(summary?.savingsProgress ?? []).length} goals</span>} />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-lg font-semibold text-gray-900">Monthly Budget</h2>
            <span className="text-sm text-gray-600">{now.toLocaleString("en-US", { month: "long" })}</span>
          </div>

          <div className="mb-6">
            <div className="flex items-baseline justify-between mb-2">
              <span className="text-3xl font-bold text-gray-900">${totalSpent.toFixed(2)}</span>
              <span className="text-gray-600">of ${totalBudget.toFixed(2)}</span>
            </div>
            <div className="w-full bg-gray-200 rounded-full h-3 overflow-hidden">
              <div className="bg-gradient-to-r from-blue-500 to-purple-600 h-full rounded-full" style={{ width: `${Math.min(100, budgetProgress)}%` }} />
            </div>
            <p className="text-sm text-gray-600 mt-2">${Math.max(0, totalBudget - totalSpent).toFixed(2)} remaining</p>
          </div>

          <div className="space-y-3">
            {(summary?.budgetUsage ?? []).map((item, idx) => (
              <div key={`${item.categoryName}-${idx}`} className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <div className="w-3 h-3 rounded-full" style={{ backgroundColor: chartColors[idx % chartColors.length] }} />
                  <span className="text-sm text-gray-700">{item.categoryName}</span>
                </div>
                <span className="text-sm font-medium text-gray-900">${Number(item.spent).toFixed(2)}</span>
              </div>
            ))}
          </div>
        </div>

        <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
          <h2 className="text-lg font-semibold text-gray-900 mb-6">Spending Distribution</h2>
          {loading ? (
            <div className="h-64 flex items-center justify-center text-gray-500">Loading chart...</div>
          ) : budgetData.length === 0 ? (
            <div className="h-64 flex items-center justify-center text-gray-500">No spending data yet.</div>
          ) : (
            <div className="h-64">
              <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                  <Pie data={budgetData} cx="50%" cy="50%" innerRadius={60} outerRadius={90} dataKey="value">
                    {budgetData.map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={entry.color} />
                    ))}
                  </Pie>
                  <Tooltip />
                </PieChart>
              </ResponsiveContainer>
            </div>
          )}
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
          <h2 className="text-lg font-semibold text-gray-900 mb-6">Recent Transactions</h2>
          <div className="space-y-4">
            {(summary?.recentTransactions ?? []).length === 0 ? (
              <p className="text-gray-500">No transactions yet.</p>
            ) : (
              (summary?.recentTransactions ?? []).map((tx) => (
                <div key={`${tx.type}-${tx.id}`} className="flex items-center justify-between">
                  <div className="flex items-center gap-3">
                    <div className={`w-10 h-10 rounded-xl flex items-center justify-center ${tx.type === "Income" ? "bg-green-100" : "bg-gray-100"}`}>
                      {tx.type === "Income" ? (
                        <ArrowUpRight className="w-5 h-5 text-green-600" />
                      ) : (
                        <ArrowDownRight className="w-5 h-5 text-gray-600" />
                      )}
                    </div>
                    <div>
                      <p className="font-medium text-gray-900 text-sm">{tx.description}</p>
                      <p className="text-xs text-gray-500">{tx.category}</p>
                    </div>
                  </div>
                  <div className="text-right">
                    <p className={`font-semibold text-sm ${tx.type === "Income" ? "text-green-600" : "text-gray-900"}`}>
                      {tx.type === "Income" ? "+" : "-"}${Math.abs(Number(tx.amount)).toFixed(2)}
                    </p>
                    <p className="text-xs text-gray-500">
                      {new Date(tx.date).toLocaleDateString("en-US", { month: "short", day: "numeric" })}
                    </p>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>

        <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
          <h2 className="text-lg font-semibold text-gray-900 mb-6">Upcoming Bills</h2>
          <div className="space-y-4">
            {(summary?.upcomingBills ?? []).length === 0 ? (
              <p className="text-gray-500">No upcoming bills.</p>
            ) : (
              (summary?.upcomingBills ?? []).map((bill, idx) => (
                <div key={`${bill.name}-${bill.dueDate}-${idx}`} className="flex items-center justify-between p-4 bg-orange-50 rounded-xl border border-orange-100">
                  <div className="flex items-center gap-3">
                    <div className="w-10 h-10 bg-orange-100 rounded-xl flex items-center justify-center">
                      <Calendar className="w-5 h-5 text-orange-600" />
                    </div>
                    <div>
                      <p className="font-medium text-gray-900 text-sm">{bill.name}</p>
                      <p className="text-xs text-gray-600">Due in {bill.daysUntilDue} days</p>
                    </div>
                  </div>
                  <p className="font-semibold text-gray-900">${Number(bill.amount).toFixed(2)}</p>
                </div>
              ))
            )}
          </div>
        </div>
      </div>

      <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
        <h2 className="text-lg font-semibold text-gray-900 mb-6">Savings Goals</h2>
        {(summary?.savingsProgress ?? []).length === 0 ? (
          <p className="text-gray-500">No savings goals yet.</p>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            {(summary?.savingsProgress ?? []).map((goal, idx) => (
              <div key={`${goal.goalName}-${idx}`} className="p-4 bg-gradient-to-br from-gray-50 to-gray-100 rounded-xl border border-gray-200">
                <h3 className="font-medium text-gray-900 mb-2">{goal.goalName}</h3>
                <div className="flex justify-between text-sm mb-1">
                  <span className="text-gray-600">${Number(goal.currentAmount).toFixed(2)}</span>
                  <span className="text-gray-600">${Number(goal.targetAmount).toFixed(2)}</span>
                </div>
                <div className="w-full bg-gray-200 rounded-full h-2 overflow-hidden">
                  <div className="bg-gradient-to-r from-blue-500 to-purple-600 h-full" style={{ width: `${Math.min(100, Number(goal.percentageComplete))}%` }} />
                </div>
                <p className="text-sm text-gray-600 mt-2">{Number(goal.percentageComplete).toFixed(0)}% complete</p>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

function TopCard({ icon, title, value, trend }: { icon: React.ReactNode; title: string; value: string; trend: React.ReactNode }) {
  return (
    <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
      <div className="flex items-center justify-between mb-4">
        <div className="w-12 h-12 bg-gray-100 rounded-xl flex items-center justify-center">{icon}</div>
        <span className="flex items-center gap-1 text-sm font-medium">{trend}</span>
      </div>
      <p className="text-gray-600 text-sm">{title}</p>
      <p className="text-2xl font-bold text-gray-900 mt-1">{value}</p>
    </div>
  );
}
