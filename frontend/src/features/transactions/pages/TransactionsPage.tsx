import { useEffect, useMemo, useState } from "react";
import { Search, ArrowUpRight, ArrowDownRight, Plus } from "lucide-react";
import { useAuth } from "../../auth/context/AuthContext";
import { ApiError } from "../../../shared/api/httpClient";
import { financeApi, type CategoryDto, type ExpenseDto, type IncomeDto } from "../../../shared/api/financeApi";

type TxType = "income" | "expense";

type UnifiedTransaction = {
  id: string;
  date: string;
  description: string;
  category: string;
  amount: number;
  type: TxType;
  person: string;
};

export default function TransactionsPage() {
  const { token } = useAuth();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filter, setFilter] = useState<"all" | TxType>("all");
  const [searchTerm, setSearchTerm] = useState("");
  const [transactions, setTransactions] = useState<UnifiedTransaction[]>([]);
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [showForm, setShowForm] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  const [formType, setFormType] = useState<TxType>("expense");
  const [formAmount, setFormAmount] = useState("");
  const [formDescription, setFormDescription] = useState("");
  const [formDate, setFormDate] = useState(new Date().toISOString().slice(0, 10));
  const [formCategoryId, setFormCategoryId] = useState<number>(0);
  const [formIsShared, setFormIsShared] = useState(true);

  const expenseCategories = useMemo(
    () => categories.filter((c) => c.type === "Expense"),
    [categories],
  );
  const incomeCategories = useMemo(
    () => categories.filter((c) => c.type === "Income"),
    [categories],
  );

  const currentTypeCategories = formType === "expense" ? expenseCategories : incomeCategories;

  const loadData = async () => {
    if (!token) return;

    setLoading(true);
    setError(null);
    try {
      const [expenses, income, allCategories] = await Promise.all([
        financeApi.getExpenses(token),
        financeApi.getIncome(token),
        financeApi.getCategories(token),
      ]);

      const mappedExpenses: UnifiedTransaction[] = (expenses as ExpenseDto[]).map((item) => ({
        id: `expense-${item.id}`,
        date: item.date,
        description: item.description,
        category: item.categoryName,
        amount: Number(item.amount),
        type: "expense",
        person: item.paidByUserName,
      }));

      const mappedIncome: UnifiedTransaction[] = (income as IncomeDto[]).map((item) => ({
        id: `income-${item.id}`,
        date: item.date,
        description: item.source,
        category: item.categoryName,
        amount: Number(item.amount),
        type: "income",
        person: item.userName,
      }));

      const merged = [...mappedIncome, ...mappedExpenses].sort(
        (a, b) => new Date(b.date).getTime() - new Date(a.date).getTime(),
      );

      setTransactions(merged);
      setCategories(allCategories);

      const defaultExpenseCategory = allCategories.find((c) => c.type === "Expense");
      if (defaultExpenseCategory && formCategoryId === 0) {
        setFormCategoryId(defaultExpenseCategory.id);
      }
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to load transactions.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadData();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token]);

  useEffect(() => {
    const first = currentTypeCategories[0];
    if (first && !currentTypeCategories.some((c) => c.id === formCategoryId)) {
      setFormCategoryId(first.id);
    }
  }, [currentTypeCategories, formCategoryId]);

  const filteredTransactions = transactions.filter((t) => {
    const matchesFilter = filter === "all" || t.type === filter;
    const matchesSearch =
      t.description.toLowerCase().includes(searchTerm.toLowerCase()) ||
      t.category.toLowerCase().includes(searchTerm.toLowerCase());
    return matchesFilter && matchesSearch;
  });

  const totalIncome = transactions
    .filter((t) => t.type === "income")
    .reduce((sum, t) => sum + t.amount, 0);
  const totalExpenses = transactions
    .filter((t) => t.type === "expense")
    .reduce((sum, t) => sum + t.amount, 0);

  const submitNewTransaction = async () => {
    if (!token) return;

    if (!formAmount || !formDescription || !formDate || formCategoryId <= 0) {
      setError("Please fill all transaction fields.");
      return;
    }

    setSubmitting(true);
    setError(null);
    try {
      if (formType === "expense") {
        await financeApi.createExpense(token, {
          amount: Number(formAmount),
          description: formDescription,
          date: new Date(formDate).toISOString(),
          categoryId: formCategoryId,
          isShared: formIsShared,
        });
      } else {
        await financeApi.createIncome(token, {
          amount: Number(formAmount),
          source: formDescription,
          date: new Date(formDate).toISOString(),
          categoryId: formCategoryId,
        });
      }

      setShowForm(false);
      setFormAmount("");
      setFormDescription("");
      await loadData();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to create transaction.");
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="max-w-7xl mx-auto space-y-6">
      <div className="flex items-center justify-between flex-wrap gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Transactions</h1>
          <p className="text-gray-600 mt-1">Live income and expense history from your household</p>
        </div>
        <button
          onClick={() => setShowForm((v) => !v)}
          className="flex items-center gap-2 px-4 py-2 bg-gradient-to-r from-blue-500 to-purple-600 text-white rounded-xl hover:shadow-lg transition-all"
        >
          <Plus className="w-5 h-5" />
          Add Transaction
        </button>
      </div>

      {showForm && (
        <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm grid grid-cols-1 md:grid-cols-6 gap-3">
          <select
            value={formType}
            onChange={(e) => setFormType(e.target.value as TxType)}
            className="px-3 py-2 border border-gray-200 rounded-xl"
          >
            <option value="expense">Expense</option>
            <option value="income">Income</option>
          </select>
          <input
            value={formAmount}
            onChange={(e) => setFormAmount(e.target.value)}
            type="number"
            min="0"
            step="0.01"
            placeholder="Amount"
            className="px-3 py-2 border border-gray-200 rounded-xl"
          />
          <input
            value={formDescription}
            onChange={(e) => setFormDescription(e.target.value)}
            placeholder={formType === "expense" ? "Description" : "Source"}
            className="px-3 py-2 border border-gray-200 rounded-xl"
          />
          <input
            value={formDate}
            onChange={(e) => setFormDate(e.target.value)}
            type="date"
            className="px-3 py-2 border border-gray-200 rounded-xl"
          />
          <select
            value={formCategoryId}
            onChange={(e) => setFormCategoryId(Number(e.target.value))}
            className="px-3 py-2 border border-gray-200 rounded-xl"
          >
            {currentTypeCategories.map((c) => (
              <option key={c.id} value={c.id}>
                {c.name}
              </option>
            ))}
          </select>
          <button
            onClick={() => void submitNewTransaction()}
            disabled={submitting}
            className="px-4 py-2 bg-blue-600 text-white rounded-xl hover:bg-blue-700 disabled:opacity-60"
          >
            {submitting ? "Saving..." : "Save"}
          </button>

          {formType === "expense" && (
            <label className="md:col-span-6 text-sm text-gray-700 flex items-center gap-2">
              <input
                type="checkbox"
                checked={formIsShared}
                onChange={(e) => setFormIsShared(e.target.checked)}
              />
              Shared expense
            </label>
          )}
        </div>
      )}

      {error && <div className="p-3 rounded-xl bg-red-50 border border-red-200 text-red-700">{error}</div>}

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
          <div className="flex items-center gap-3 mb-2">
            <div className="w-10 h-10 bg-green-100 rounded-xl flex items-center justify-center">
              <ArrowUpRight className="w-5 h-5 text-green-600" />
            </div>
            <p className="text-gray-600 text-sm">Total Income</p>
          </div>
          <p className="text-2xl font-bold text-green-600">+${totalIncome.toFixed(2)}</p>
        </div>
        <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
          <div className="flex items-center gap-3 mb-2">
            <div className="w-10 h-10 bg-red-100 rounded-xl flex items-center justify-center">
              <ArrowDownRight className="w-5 h-5 text-red-600" />
            </div>
            <p className="text-gray-600 text-sm">Total Expenses</p>
          </div>
          <p className="text-2xl font-bold text-red-600">-${totalExpenses.toFixed(2)}</p>
        </div>
        <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
          <p className="text-gray-600 text-sm mb-1">Net Balance</p>
          <p className="text-2xl font-bold text-gray-900">${(totalIncome - totalExpenses).toFixed(2)}</p>
        </div>
      </div>

      <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
        <div className="flex flex-col sm:flex-row gap-4">
          <div className="flex-1 relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
            <input
              type="text"
              placeholder="Search transactions..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-gray-200 rounded-xl"
            />
          </div>
          <div className="flex gap-2">
            {(["all", "income", "expense"] as const).map((value) => (
              <button
                key={value}
                onClick={() => setFilter(value)}
                className={`px-4 py-2 rounded-xl font-medium ${
                  filter === value
                    ? "bg-gradient-to-r from-blue-500 to-purple-600 text-white"
                    : "bg-gray-100 text-gray-700"
                }`}
              >
                {value[0].toUpperCase() + value.slice(1)}
              </button>
            ))}
          </div>
        </div>
      </div>

      <div className="bg-white rounded-2xl border border-gray-200 shadow-sm overflow-hidden">
        {loading ? (
          <div className="p-10 text-center text-gray-500">Loading transactions...</div>
        ) : filteredTransactions.length === 0 ? (
          <div className="p-10 text-center text-gray-500">No transactions found.</div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-gray-50 border-b border-gray-200">
                <tr>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase">Date</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase">Description</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase">Category</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase">Person</th>
                  <th className="px-6 py-4 text-right text-xs font-semibold text-gray-600 uppercase">Amount</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200">
                {filteredTransactions.map((transaction) => (
                  <tr key={transaction.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4 text-sm text-gray-600">
                      {new Date(transaction.date).toLocaleDateString("en-US", {
                        month: "short",
                        day: "numeric",
                        year: "numeric",
                      })}
                    </td>
                    <td className="px-6 py-4 text-sm font-medium text-gray-900">{transaction.description}</td>
                    <td className="px-6 py-4 text-sm text-gray-700">{transaction.category}</td>
                    <td className="px-6 py-4 text-sm text-gray-600">{transaction.person}</td>
                    <td className="px-6 py-4 text-right">
                      <span className={`font-semibold ${transaction.type === "income" ? "text-green-600" : "text-gray-900"}`}>
                        {transaction.type === "income" ? "+" : "-"}${Math.abs(transaction.amount).toFixed(2)}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}
