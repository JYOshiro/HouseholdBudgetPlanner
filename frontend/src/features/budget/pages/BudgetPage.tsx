import { useEffect, useMemo, useState } from "react";
import { Pencil, Plus, Save, Trash2, X } from "lucide-react";
import { useAuth } from "../../auth/context/AuthContext";
import { ApiError } from "../../../shared/api/httpClient";
import { financeApi, type BudgetDto, type CategoryDto } from "../../../shared/api/financeApi";

export default function BudgetPage() {
  const { token } = useAuth();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [budgets, setBudgets] = useState<BudgetDto[]>([]);
  const [expenseCategories, setExpenseCategories] = useState<CategoryDto[]>([]);
  const [showForm, setShowForm] = useState(false);
  const [saving, setSaving] = useState(false);
  const [editingCategoryId, setEditingCategoryId] = useState<number | null>(null);
  const [editingCategoryName, setEditingCategoryName] = useState("");
  const [editingCategoryColor, setEditingCategoryColor] = useState("#3b82f6");
  const [editingBudgetCategoryId, setEditingBudgetCategoryId] = useState<number | null>(null);
  const [editingBudgetAmount, setEditingBudgetAmount] = useState("");

  const [categoryName, setCategoryName] = useState("");
  const [categoryColor, setCategoryColor] = useState("#3b82f6");
  const [monthlyBudget, setMonthlyBudget] = useState("");

  const now = useMemo(() => new Date(), []);
  const year = now.getFullYear();
  const month = now.getMonth() + 1;
  // Use UTC month start to avoid timezone offsets pushing the date into the previous month.
  const firstDayOfMonthIso = useMemo(() => new Date(Date.UTC(year, month - 1, 1)).toISOString(), [month, year]);

  const budgetsByCategoryId = useMemo(
    () => new Map(budgets.map((budget) => [budget.categoryId, budget])),
    [budgets],
  );

  const loadData = async () => {
    if (!token) return;
    setLoading(true);
    setError(null);
    try {
      const [budgetData, categories] = await Promise.all([
        financeApi.getBudgets(token, year, month),
        financeApi.getCategories(token, "Expense"),
      ]);
      setBudgets(budgetData);
      setExpenseCategories(categories);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to load budget data.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadData();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token]);

  const totalBudgeted = budgets.reduce((sum, b) => sum + Number(b.amount), 0);
  const totalSpent = budgets.reduce((sum, b) => sum + Number(b.currentSpent), 0);
  const totalRemaining = totalBudgeted - totalSpent;

  const handleCreateCategory = async () => {
    if (!token) return;
    if (!categoryName.trim()) {
      setError("Category name is required.");
      return;
    }

    setSaving(true);
    setError(null);

    try {
      const created = await financeApi.createCategory(token, {
        name: categoryName.trim(),
        type: "Expense",
        color: categoryColor,
      });

      if (Number(monthlyBudget) > 0) {
        await financeApi.createBudget(token, {
          amount: Number(monthlyBudget),
          categoryId: created.id,
          month: firstDayOfMonthIso,
        });
      }

      setShowForm(false);
      setCategoryName("");
      setMonthlyBudget("");
      await loadData();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to create category.");
    } finally {
      setSaving(false);
    }
  };

  const startCategoryEdit = (category: CategoryDto) => {
    setEditingCategoryId(category.id);
    setEditingCategoryName(category.name);
    setEditingCategoryColor(category.color ?? "#3b82f6");
  };

  const cancelCategoryEdit = () => {
    setEditingCategoryId(null);
    setEditingCategoryName("");
    setEditingCategoryColor("#3b82f6");
  };

  const handleUpdateCategory = async () => {
    if (!token || editingCategoryId === null) return;
    if (!editingCategoryName.trim()) {
      setError("Category name is required.");
      return;
    }

    setSaving(true);
    setError(null);

    try {
      await financeApi.updateCategory(token, editingCategoryId, {
        name: editingCategoryName.trim(),
        color: editingCategoryColor,
      });

      cancelCategoryEdit();
      await loadData();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to update category.");
    } finally {
      setSaving(false);
    }
  };

  const handleDeleteCategory = async (category: CategoryDto) => {
    if (!token) return;
    if (!window.confirm(`Delete category \"${category.name}\"?`)) return;

    setSaving(true);
    setError(null);

    try {
      await financeApi.deleteCategory(token, category.id);
      await loadData();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to delete category.");
    } finally {
      setSaving(false);
    }
  };

  const startBudgetEdit = (categoryId: number) => {
    const existingBudget = budgetsByCategoryId.get(categoryId);
    setEditingBudgetCategoryId(categoryId);
    setEditingBudgetAmount(existingBudget ? String(existingBudget.amount) : "");
  };

  const cancelBudgetEdit = () => {
    setEditingBudgetCategoryId(null);
    setEditingBudgetAmount("");
  };

  const handleSaveBudget = async (categoryId: number) => {
    if (!token) return;

    const amount = Number(editingBudgetAmount);
    if (!Number.isFinite(amount) || amount <= 0) {
      setError("Budget amount must be greater than zero.");
      return;
    }

    setSaving(true);
    setError(null);

    try {
      const existingBudget = budgetsByCategoryId.get(categoryId);

      if (existingBudget) {
        await financeApi.updateBudget(token, existingBudget.id, { amount });
      } else {
        await financeApi.createBudget(token, {
          amount,
          categoryId,
          month: firstDayOfMonthIso,
        });
      }

      cancelBudgetEdit();
      await loadData();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to save budget.");
    } finally {
      setSaving(false);
    }
  };

  const handleDeleteBudget = async (budget: BudgetDto) => {
    if (!token) return;
    if (!window.confirm(`Remove budget for \"${budget.categoryName}\"?`)) return;

    setSaving(true);
    setError(null);

    try {
      await financeApi.deleteBudget(token, budget.id);
      await loadData();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to delete budget.");
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="max-w-7xl mx-auto space-y-6">
      <div className="flex items-center justify-between flex-wrap gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Budget Overview</h1>
          <p className="text-gray-600 mt-1">Live budget data for {now.toLocaleString("en-US", { month: "long", year: "numeric" })}</p>
        </div>
        <button
          onClick={() => setShowForm((v) => !v)}
          className="flex items-center gap-2 px-4 py-2 bg-gradient-to-r from-blue-500 to-purple-600 text-white rounded-xl hover:shadow-lg"
        >
          <Plus className="w-5 h-5" />
          Add Category
        </button>
      </div>

      {showForm && (
        <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm grid grid-cols-1 md:grid-cols-4 gap-3">
          <input
            value={categoryName}
            onChange={(e) => setCategoryName(e.target.value)}
            placeholder="Category name"
            className="px-3 py-2 border border-gray-200 rounded-xl"
          />
          <input
            type="color"
            value={categoryColor}
            onChange={(e) => setCategoryColor(e.target.value)}
            className="h-10 border border-gray-200 rounded-xl"
          />
          <input
            type="number"
            min="0"
            step="0.01"
            value={monthlyBudget}
            onChange={(e) => setMonthlyBudget(e.target.value)}
            placeholder="Optional monthly budget"
            className="px-3 py-2 border border-gray-200 rounded-xl"
          />
          <button
            onClick={() => void handleCreateCategory()}
            disabled={saving}
            className="px-4 py-2 bg-blue-600 text-white rounded-xl disabled:opacity-60"
          >
            {saving ? "Saving..." : "Save"}
          </button>
        </div>
      )}

      {error && <div className="p-3 rounded-xl bg-red-50 border border-red-200 text-red-700">{error}</div>}

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
          <p className="text-gray-600 text-sm mb-2">Total Budgeted</p>
          <p className="text-3xl font-bold text-gray-900">${totalBudgeted.toFixed(2)}</p>
        </div>
        <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
          <p className="text-gray-600 text-sm mb-2">Total Spent</p>
          <p className="text-3xl font-bold text-blue-600">${totalSpent.toFixed(2)}</p>
        </div>
        <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
          <p className="text-gray-600 text-sm mb-2">Remaining</p>
          <p className={`text-3xl font-bold ${totalRemaining >= 0 ? "text-green-600" : "text-red-600"}`}>
            ${totalRemaining.toFixed(2)}
          </p>
        </div>
      </div>

      <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
        <h2 className="text-lg font-semibold text-gray-900 mb-4">Expense Categories ({expenseCategories.length})</h2>
        <div className="space-y-3">
          {expenseCategories.map((c) => (
            <div key={c.id} className="rounded-xl border border-gray-200 p-4">
              <div className="flex flex-col gap-3 lg:flex-row lg:items-center lg:justify-between">
                <div className="flex items-center gap-3">
                  <span
                    className="inline-flex px-3 py-1 text-sm rounded-full border"
                    style={{ borderColor: c.color ?? "#d1d5db", color: c.color ?? "#374151" }}
                  >
                    {c.name}
                  </span>
                  <span className="text-xs text-gray-500">{c.isSystemDefault ? "Default category" : "Custom category"}</span>
                </div>

                <div className="flex flex-wrap items-center gap-2">
                  {editingBudgetCategoryId === c.id ? (
                    <>
                      <input
                        type="number"
                        min="0"
                        step="0.01"
                        value={editingBudgetAmount}
                        onChange={(e) => setEditingBudgetAmount(e.target.value)}
                        placeholder="Budget amount"
                        className="w-40 px-3 py-2 border border-gray-200 rounded-xl"
                      />
                      <button
                        onClick={() => void handleSaveBudget(c.id)}
                        disabled={saving}
                        className="inline-flex items-center gap-2 px-3 py-2 bg-blue-600 text-white rounded-xl disabled:opacity-60"
                      >
                        <Save className="w-4 h-4" />
                        Save Budget
                      </button>
                      <button
                        onClick={cancelBudgetEdit}
                        className="inline-flex items-center gap-2 px-3 py-2 border border-gray-200 text-gray-700 rounded-xl"
                      >
                        <X className="w-4 h-4" />
                        Cancel
                      </button>
                    </>
                  ) : (
                    <button
                      onClick={() => startBudgetEdit(c.id)}
                      className="inline-flex items-center gap-2 px-3 py-2 border border-gray-200 text-gray-700 rounded-xl hover:bg-gray-50"
                    >
                      <Pencil className="w-4 h-4" />
                      {budgetsByCategoryId.has(c.id) ? "Edit Budget" : "Set Budget"}
                    </button>
                  )}

                  {!c.isSystemDefault && editingCategoryId !== c.id && (
                    <>
                      <button
                        onClick={() => startCategoryEdit(c)}
                        className="inline-flex items-center gap-2 px-3 py-2 border border-gray-200 text-gray-700 rounded-xl hover:bg-gray-50"
                      >
                        <Pencil className="w-4 h-4" />
                        Edit Category
                      </button>
                      <button
                        onClick={() => void handleDeleteCategory(c)}
                        disabled={saving}
                        className="inline-flex items-center gap-2 px-3 py-2 border border-red-200 text-red-600 rounded-xl hover:bg-red-50 disabled:opacity-60"
                      >
                        <Trash2 className="w-4 h-4" />
                        Delete Category
                      </button>
                    </>
                  )}
                </div>
              </div>

              {editingCategoryId === c.id && (
                <div className="mt-4 grid grid-cols-1 md:grid-cols-4 gap-3">
                  <input
                    value={editingCategoryName}
                    onChange={(e) => setEditingCategoryName(e.target.value)}
                    placeholder="Category name"
                    className="px-3 py-2 border border-gray-200 rounded-xl"
                  />
                  <input
                    type="color"
                    value={editingCategoryColor}
                    onChange={(e) => setEditingCategoryColor(e.target.value)}
                    className="h-10 border border-gray-200 rounded-xl"
                  />
                  <button
                    onClick={() => void handleUpdateCategory()}
                    disabled={saving}
                    className="inline-flex items-center justify-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-xl disabled:opacity-60"
                  >
                    <Save className="w-4 h-4" />
                    Save Category
                  </button>
                  <button
                    onClick={cancelCategoryEdit}
                    className="inline-flex items-center justify-center gap-2 px-4 py-2 border border-gray-200 text-gray-700 rounded-xl"
                  >
                    <X className="w-4 h-4" />
                    Cancel
                  </button>
                </div>
              )}
            </div>
          ))}
        </div>
      </div>

      <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
        <h2 className="text-lg font-semibold text-gray-900 mb-4">Category Budgets</h2>
        {loading ? (
          <p className="text-gray-500">Loading budgets...</p>
        ) : budgets.length === 0 ? (
          <p className="text-gray-500">No budgets set for this month yet.</p>
        ) : (
          <div className="space-y-4">
            {budgets.map((b) => (
              <div key={b.id} className="p-4 bg-gray-50 rounded-xl border border-gray-100">
                <div className="flex items-center justify-between mb-2">
                  <h3 className="font-semibold text-gray-900">{b.categoryName}</h3>
                  <div className="flex items-center gap-3">
                    <span className="text-sm text-gray-700">
                      ${Number(b.currentSpent).toFixed(2)} / ${Number(b.amount).toFixed(2)}
                    </span>
                    <button
                      onClick={() => startBudgetEdit(b.categoryId)}
                      className="inline-flex items-center gap-1 text-sm text-blue-600 hover:text-blue-700"
                    >
                      <Pencil className="w-4 h-4" />
                      Edit
                    </button>
                    <button
                      onClick={() => void handleDeleteBudget(b)}
                      disabled={saving}
                      className="inline-flex items-center gap-1 text-sm text-red-600 hover:text-red-700 disabled:opacity-60"
                    >
                      <Trash2 className="w-4 h-4" />
                      Remove
                    </button>
                  </div>
                </div>
                <div className="w-full h-2 rounded-full bg-gray-200 overflow-hidden">
                  <div
                    className={`h-full ${Number(b.percentageUsed) > 100 ? "bg-red-500" : "bg-blue-600"}`}
                    style={{ width: `${Math.min(100, Number(b.percentageUsed))}%` }}
                  />
                </div>
                <p className="mt-2 text-sm text-gray-600">
                  {Number(b.percentageUsed).toFixed(0)}% used, remaining ${Number(b.remaining).toFixed(2)}
                </p>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
