import { useEffect, useMemo, useState } from "react";
import { DollarSign, PieChart as PieChartIcon } from "lucide-react";
import { PieChart, Pie, Cell, ResponsiveContainer, BarChart, Bar, XAxis, YAxis, Tooltip } from "recharts";
import { useAuth } from "../../auth/context/AuthContext";
import { ApiError } from "../../../shared/api/httpClient";
import {
  financeApi,
  type ExpenseDto,
  type HouseholdDto,
  type HouseholdMemberDto,
  type IncomeDto,
} from "../../../shared/api/financeApi";

const memberColors = ["#3b82f6", "#8b5cf6", "#10b981", "#f59e0b", "#ec4899", "#14b8a6"];

export default function HouseholdPage() {
  const { token } = useAuth();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [household, setHousehold] = useState<HouseholdDto | null>(null);
  const [members, setMembers] = useState<HouseholdMemberDto[]>([]);
  const [expenses, setExpenses] = useState<ExpenseDto[]>([]);
  const [income, setIncome] = useState<IncomeDto[]>([]);

  const [personFilter, setPersonFilter] = useState<"all" | number>("all");

  const loadData = async () => {
    if (!token) return;

    setLoading(true);
    setError(null);
    try {
      const [householdData, memberData, expenseData, incomeData] = await Promise.all([
        financeApi.getHousehold(token),
        financeApi.getHouseholdMembers(token),
        financeApi.getExpenses(token),
        financeApi.getIncome(token),
      ]);

      setHousehold(householdData);
      setMembers(memberData);
      setExpenses(expenseData);
      setIncome(incomeData);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to load household data.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadData();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token]);

  const contributionByMember = useMemo(() => {
    const map = new Map<number, { total: number; transactionCount: number; name: string }>();

    members.forEach((member) => {
      map.set(member.id, { total: 0, transactionCount: 0, name: member.fullName });
    });

    expenses.forEach((exp) => {
      const entry = map.get(exp.paidByUserId);
      if (entry) {
        entry.total += Number(exp.amount);
        entry.transactionCount += 1;
      }
    });

    income.forEach((inc) => {
      const entry = map.get(inc.userId);
      if (entry) {
        entry.total += Number(inc.amount);
        entry.transactionCount += 1;
      }
    });

    return [...map.entries()].map(([memberId, stats], idx) => ({
      memberId,
      name: stats.name,
      initials: stats.name
        .split(" ")
        .map((part) => part[0])
        .join("")
        .slice(0, 2)
        .toUpperCase(),
      totalContributed: stats.total,
      transactionCount: stats.transactionCount,
      color: memberColors[idx % memberColors.length],
    }));
  }, [members, expenses, income]);

  const contributionTotal = contributionByMember.reduce((sum, m) => sum + m.totalContributed, 0);

  const sharedExpenses = useMemo(
    () =>
      expenses
        .filter((exp) => exp.isShared)
        .sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime()),
    [expenses],
  );

  const personalExpenses = useMemo(
    () =>
      expenses
        .filter((exp) => !exp.isShared)
        .sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime()),
    [expenses],
  );

  const filteredSharedExpenses =
    personFilter === "all" ? sharedExpenses : sharedExpenses.filter((exp) => exp.paidByUserId === personFilter);

  const categoryBreakdown = useMemo(() => {
    const categoryMap = new Map<string, Record<number, number>>();

    sharedExpenses.forEach((exp) => {
      if (!categoryMap.has(exp.categoryName)) {
        categoryMap.set(exp.categoryName, {});
      }
      const bucket = categoryMap.get(exp.categoryName)!;
      bucket[exp.paidByUserId] = (bucket[exp.paidByUserId] ?? 0) + Number(exp.amount);
    });

    const topCategories = [...categoryMap.entries()]
      .map(([category, byMember]) => ({
        category,
        total: Object.values(byMember).reduce((s, v) => s + v, 0),
        byMember,
      }))
      .sort((a, b) => b.total - a.total)
      .slice(0, 6);

    return topCategories.map((item) => {
      const base: Record<string, string | number> = { category: item.category };
      contributionByMember.forEach((member) => {
        base[`member-${member.memberId}`] = item.byMember[member.memberId] ?? 0;
      });
      return base;
    });
  }, [sharedExpenses, contributionByMember]);

  const contributionPieData = contributionByMember.map((member) => ({
    name: member.name.split(" ")[0],
    amount: member.totalContributed,
    color: member.color,
  }));

  const totalSharedExpenses = sharedExpenses.reduce((sum, exp) => sum + Number(exp.amount), 0);

  return (
    <div className="max-w-7xl mx-auto space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Household Finance</h1>
        <p className="text-gray-600 mt-1">
          {household ? `${household.name} • ` : ""}Live shared expenses and member contribution insights
        </p>
      </div>

      {error && <div className="p-3 rounded-xl bg-red-50 border border-red-200 text-red-700">{error}</div>}

      {loading ? (
        <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm text-gray-500">
          Loading household data...
        </div>
      ) : (
        <>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {contributionByMember.map((member) => {
              const percentage = contributionTotal > 0 ? (member.totalContributed / contributionTotal) * 100 : 0;
              return (
                <div key={member.memberId} className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
                  <div className="flex items-center gap-4 mb-4">
                    <div
                      className="w-16 h-16 rounded-full flex items-center justify-center text-white text-xl font-bold"
                      style={{ backgroundColor: member.color }}
                    >
                      {member.initials}
                    </div>
                    <div className="flex-1">
                      <h3 className="font-semibold text-gray-900 text-lg">{member.name}</h3>
                      <p className="text-sm text-gray-600">{member.transactionCount} transactions recorded</p>
                    </div>
                  </div>

                  <div className="space-y-3">
                    <div>
                      <div className="flex items-baseline justify-between mb-2">
                        <span className="text-sm text-gray-600">Total Contributed</span>
                        <span className="text-2xl font-bold text-gray-900">${member.totalContributed.toFixed(2)}</span>
                      </div>
                      <div className="w-full bg-gray-200 rounded-full h-2">
                        <div
                          className="h-full rounded-full"
                          style={{ width: `${percentage}%`, backgroundColor: member.color }}
                        />
                      </div>
                      <p className="text-sm text-gray-600 mt-1">{percentage.toFixed(0)}% of household total</p>
                    </div>
                  </div>
                </div>
              );
            })}
          </div>

          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
              <h2 className="text-lg font-semibold text-gray-900 mb-6">Contribution Distribution</h2>
              {contributionPieData.length === 0 ? (
                <p className="text-gray-500">No contribution data yet.</p>
              ) : (
                <div className="h-64">
                  <ResponsiveContainer width="100%" height="100%">
                    <PieChart>
                      <Pie
                        data={contributionPieData}
                        cx="50%"
                        cy="50%"
                        innerRadius={60}
                        outerRadius={90}
                        paddingAngle={5}
                        dataKey="amount"
                        label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`}
                      >
                        {contributionPieData.map((entry, index) => (
                          <Cell key={`cell-${index}`} fill={entry.color} />
                        ))}
                      </Pie>
                      <Tooltip />
                    </PieChart>
                  </ResponsiveContainer>
                </div>
              )}
            </div>

            <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
              <h2 className="text-lg font-semibold text-gray-900 mb-6">Shared Spending by Category</h2>
              {categoryBreakdown.length === 0 ? (
                <p className="text-gray-500">No shared expenses to chart yet.</p>
              ) : (
                <div className="h-64">
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart data={categoryBreakdown}>
                      <XAxis dataKey="category" stroke="#9ca3af" fontSize={12} />
                      <YAxis stroke="#9ca3af" fontSize={12} />
                      <Tooltip />
                      {contributionByMember.map((member) => (
                        <Bar
                          key={member.memberId}
                          dataKey={`member-${member.memberId}`}
                          fill={member.color}
                          radius={[8, 8, 0, 0]}
                        />
                      ))}
                    </BarChart>
                  </ResponsiveContainer>
                </div>
              )}
            </div>
          </div>

          <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
            <div className="flex items-center justify-between mb-6">
              <div>
                <h2 className="text-lg font-semibold text-gray-900">Shared Expenses</h2>
                <p className="text-sm text-gray-600 mt-1">Total: ${totalSharedExpenses.toFixed(2)}</p>
              </div>
              <div className="flex gap-2 flex-wrap">
                <button
                  onClick={() => setPersonFilter("all")}
                  className={`px-4 py-2 rounded-lg text-sm font-medium ${
                    personFilter === "all" ? "bg-blue-100 text-blue-700" : "bg-gray-100 text-gray-700 hover:bg-gray-200"
                  }`}
                >
                  All
                </button>
                {contributionByMember.map((member) => (
                  <button
                    key={member.memberId}
                    onClick={() => setPersonFilter(member.memberId)}
                    className={`px-4 py-2 rounded-lg text-sm font-medium ${
                      personFilter === member.memberId
                        ? "bg-blue-100 text-blue-700"
                        : "bg-gray-100 text-gray-700 hover:bg-gray-200"
                    }`}
                  >
                    {member.name.split(" ")[0]}
                  </button>
                ))}
              </div>
            </div>

            <div className="space-y-3">
              {filteredSharedExpenses.length === 0 ? (
                <p className="text-gray-500">No shared expenses found.</p>
              ) : (
                filteredSharedExpenses.map((expense) => (
                  <div key={expense.id} className="flex items-center justify-between p-4 bg-gray-50 rounded-xl">
                    <div className="flex items-center gap-3 flex-1">
                      <div className="w-10 h-10 rounded-xl flex items-center justify-center bg-blue-100">
                        <DollarSign className="w-5 h-5 text-blue-600" />
                      </div>
                      <div className="flex-1">
                        <p className="font-medium text-gray-900">{expense.description}</p>
                        <p className="text-sm text-gray-600">
                          {expense.categoryName} • Paid by {expense.paidByUserName}
                        </p>
                      </div>
                    </div>
                    <div className="text-right">
                      <p className="font-semibold text-gray-900">${Number(expense.amount).toFixed(2)}</p>
                      <p className="text-xs text-gray-500">
                        {new Date(expense.date).toLocaleDateString("en-US", {
                          month: "short",
                          day: "numeric",
                          year: "numeric",
                        })}
                      </p>
                    </div>
                  </div>
                ))
              )}
            </div>
          </div>

          <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
            <h2 className="text-lg font-semibold text-gray-900 mb-6">Personal (Non-shared) Expenses</h2>
            <div className="space-y-3">
              {personalExpenses.length === 0 ? (
                <p className="text-gray-500">No personal expenses found.</p>
              ) : (
                personalExpenses.map((expense) => (
                  <div key={expense.id} className="flex items-center justify-between p-4 bg-gray-50 rounded-xl">
                    <div className="flex items-center gap-3 flex-1">
                      <div className="w-10 h-10 rounded-xl flex items-center justify-center bg-purple-100">
                        <PieChartIcon className="w-5 h-5 text-purple-600" />
                      </div>
                      <div className="flex-1">
                        <p className="font-medium text-gray-900">{expense.description}</p>
                        <p className="text-sm text-gray-600">
                          {expense.categoryName} • {expense.paidByUserName}'s expense
                        </p>
                      </div>
                    </div>
                    <div className="text-right">
                      <p className="font-semibold text-gray-900">${Number(expense.amount).toFixed(2)}</p>
                      <p className="text-xs text-gray-500">
                        {new Date(expense.date).toLocaleDateString("en-US", {
                          month: "short",
                          day: "numeric",
                          year: "numeric",
                        })}
                      </p>
                    </div>
                  </div>
                ))
              )}
            </div>
          </div>
        </>
      )}
    </div>
  );
}
