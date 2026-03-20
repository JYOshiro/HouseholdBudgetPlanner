import { useEffect, useState } from "react";
import { Target, Plus, TrendingUp, Calendar, DollarSign, CheckCircle2, Pencil, Save, X, Archive, RotateCcw, Trash2 } from "lucide-react";
import { useAuth } from "../../auth/context/AuthContext";
import { ApiError } from "../../../shared/api/httpClient";
import { financeApi, type SavingsGoalDto } from "../../../shared/api/financeApi";

export default function SavingsPage() {
  const { token } = useAuth();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [goals, setGoals] = useState<SavingsGoalDto[]>([]);
  const [showForm, setShowForm] = useState(false);
  const [saving, setSaving] = useState(false);

  // New goal form state
  const [name, setName] = useState("");
  const [targetAmount, setTargetAmount] = useState("");
  const [targetDate, setTargetDate] = useState("");
  const [priority, setPriority] = useState("Normal");

  // Edit goal state
  const [editingGoalId, setEditingGoalId] = useState<number | null>(null);
  const [editName, setEditName] = useState("");
  const [editTargetAmount, setEditTargetAmount] = useState("");
  const [editTargetDate, setEditTargetDate] = useState("");
  const [editPriority, setEditPriority] = useState("Normal");

  const loadData = async () => {
    if (!token) return;
    setLoading(true);
    setError(null);
    try {
      const data = await financeApi.getSavingsGoals(token);
      setGoals(data);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to load savings goals.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadData();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token]);

  const createGoal = async () => {
    if (!token) return;
    if (!name.trim() || !targetAmount) {
      setError("Goal name and target amount are required.");
      return;
    }
    setSaving(true);
    setError(null);
    try {
      await financeApi.createSavingsGoal(token, {
        name: name.trim(),
        targetAmount: Number(targetAmount),
        targetDate: targetDate ? new Date(targetDate).toISOString() : undefined,
        priority,
      });
      setShowForm(false);
      setName("");
      setTargetAmount("");
      setTargetDate("");
      setPriority("Normal");
      await loadData();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to create savings goal.");
    } finally {
      setSaving(false);
    }
  };

  const startEditGoal = (goal: SavingsGoalDto) => {
    setEditingGoalId(goal.id);
    setEditName(goal.name);
    setEditTargetAmount(String(goal.targetAmount));
    setEditTargetDate(goal.targetDate ? goal.targetDate.substring(0, 10) : "");
    setEditPriority(goal.priority);
  };

  const cancelEditGoal = () => {
    setEditingGoalId(null);
    setEditName("");
    setEditTargetAmount("");
    setEditTargetDate("");
    setEditPriority("Normal");
  };

  const saveEditGoal = async (goalId: number) => {
    if (!token) return;
    if (!editName.trim() || !editTargetAmount) {
      setError("Goal name and target amount are required.");
      return;
    }
    setSaving(true);
    setError(null);
    try {
      await financeApi.updateSavingsGoal(token, goalId, {
        name: editName.trim(),
        targetAmount: Number(editTargetAmount),
        targetDate: editTargetDate ? new Date(editTargetDate).toISOString() : null,
        priority: editPriority,
      });
      cancelEditGoal();
      await loadData();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to update savings goal.");
    } finally {
      setSaving(false);
    }
  };

  const archiveGoal = async (goal: SavingsGoalDto) => {
    if (!token) return;
    if (!window.confirm(`Archive goal "${goal.name}"? It will be hidden from active goals.`)) return;
    setSaving(true);
    setError(null);
    try {
      await financeApi.updateSavingsGoal(token, goal.id, { status: "Archived" });
      await loadData();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to archive goal.");
    } finally {
      setSaving(false);
    }
  };

  const reopenGoal = async (goal: SavingsGoalDto) => {
    if (!token) return;
    setSaving(true);
    setError(null);
    try {
      await financeApi.updateSavingsGoal(token, goal.id, { status: "Active" });
      await loadData();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to reopen goal.");
    } finally {
      setSaving(false);
    }
  };

  const deleteGoal = async (goal: SavingsGoalDto) => {
    if (!token) return;
    if (!window.confirm(`Permanently delete goal "${goal.name}"? This cannot be undone.`)) return;
    setSaving(true);
    setError(null);
    try {
      await financeApi.deleteSavingsGoal(token, goal.id);
      await loadData();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to delete goal.");
    } finally {
      setSaving(false);
    }
  };

  const addContribution = async (goalId: number) => {
    if (!token) return;
    const input = window.prompt("Contribution amount", "50");
    if (!input) return;
    const amount = Number(input);
    if (!Number.isFinite(amount) || amount <= 0) {
      setError("Contribution amount must be a positive number.");
      return;
    }
    setError(null);
    try {
      await financeApi.createGoalContribution(token, goalId, {
        amount,
        contributionDate: new Date().toISOString(),
      });
      await loadData();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to add contribution.");
    }
  };

  const activeGoals = goals.filter((g) => g.status === "Active");
  const completedGoals = goals.filter((g) => g.status === "Completed");
  const archivedGoals = goals.filter((g) => g.status === "Archived");

  const totalSaved = activeGoals.reduce((sum, g) => sum + Number(g.currentAmount), 0);
  const totalTarget = activeGoals.reduce((sum, g) => sum + Number(g.targetAmount), 0);
  const activeRemaining = Math.max(0, totalTarget - totalSaved);

  return (
    <div className="max-w-7xl mx-auto space-y-6">
      <div className="flex items-center justify-between flex-wrap gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Savings Goals</h1>
          <p className="text-gray-600 mt-1">Track your household savings progress</p>
        </div>
        <button
          onClick={() => setShowForm((v) => !v)}
          className="flex items-center gap-2 px-4 py-2 bg-gradient-to-r from-blue-500 to-purple-600 text-white rounded-xl hover:shadow-lg"
        >
          <Plus className="w-5 h-5" />
          Add Goal
        </button>
      </div>

      {showForm && (
        <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm grid grid-cols-1 md:grid-cols-5 gap-3">
          <input value={name} onChange={(e) => setName(e.target.value)} placeholder="Goal name" className="px-3 py-2 border border-gray-200 rounded-xl" />
          <input value={targetAmount} onChange={(e) => setTargetAmount(e.target.value)} type="number" min="0" step="0.01" placeholder="Target amount" className="px-3 py-2 border border-gray-200 rounded-xl" />
          <input value={targetDate} onChange={(e) => setTargetDate(e.target.value)} type="date" className="px-3 py-2 border border-gray-200 rounded-xl" />
          <select value={priority} onChange={(e) => setPriority(e.target.value)} className="px-3 py-2 border border-gray-200 rounded-xl">
            <option value="High">High</option>
            <option value="Normal">Normal</option>
            <option value="Low">Low</option>
          </select>
          <button onClick={() => void createGoal()} disabled={saving} className="px-4 py-2 bg-blue-600 text-white rounded-xl disabled:opacity-60">
            {saving ? "Saving..." : "Save"}
          </button>
        </div>
      )}

      {error && <div className="p-3 rounded-xl bg-red-50 border border-red-200 text-red-700">{error}</div>}

      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <SummaryCard icon={<DollarSign className="w-5 h-5 text-green-600" />} label="Total Saved (Active)" value={`$${totalSaved.toFixed(2)}`} />
        <SummaryCard icon={<Target className="w-5 h-5 text-blue-600" />} label="Total Target (Active)" value={`$${totalTarget.toFixed(2)}`} />
        <SummaryCard icon={<TrendingUp className="w-5 h-5 text-purple-600" />} label="Active Goals" value={`${activeGoals.length}`} />
        <SummaryCard icon={<Calendar className="w-5 h-5 text-orange-600" />} label="Remaining (Active)" value={`$${activeRemaining.toFixed(2)}`} />
      </div>

      {/* Active Goals */}
      <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
        <h2 className="text-lg font-semibold text-gray-900 mb-4">Active Goals ({activeGoals.length})</h2>
        {loading ? (
          <p className="text-gray-500">Loading goals...</p>
        ) : activeGoals.length === 0 ? (
          <p className="text-gray-500">No active goals. Create your first goal above.</p>
        ) : (
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
            {activeGoals.map((goal) => (
              <GoalCard
                key={goal.id}
                goal={goal}
                isEditing={editingGoalId === goal.id}
                editName={editName}
                editTargetAmount={editTargetAmount}
                editTargetDate={editTargetDate}
                editPriority={editPriority}
                saving={saving}
                onEditName={setEditName}
                onEditTargetAmount={setEditTargetAmount}
                onEditTargetDate={setEditTargetDate}
                onEditPriority={setEditPriority}
                onStartEdit={() => startEditGoal(goal)}
                onSaveEdit={() => void saveEditGoal(goal.id)}
                onCancelEdit={cancelEditGoal}
                onAddContribution={() => void addContribution(goal.id)}
                onArchive={() => void archiveGoal(goal)}
                onDelete={() => void deleteGoal(goal)}
              />
            ))}
          </div>
        )}
      </div>

      {/* Completed Goals */}
      {completedGoals.length > 0 && (
        <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
          <div className="flex items-center gap-3 mb-4">
            <CheckCircle2 className="w-5 h-5 text-green-600" />
            <h2 className="text-lg font-semibold text-gray-900">Completed Goals ({completedGoals.length})</h2>
          </div>
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
            {completedGoals.map((goal) => (
              <GoalCard
                key={goal.id}
                goal={goal}
                isEditing={editingGoalId === goal.id}
                editName={editName}
                editTargetAmount={editTargetAmount}
                editTargetDate={editTargetDate}
                editPriority={editPriority}
                saving={saving}
                onEditName={setEditName}
                onEditTargetAmount={setEditTargetAmount}
                onEditTargetDate={setEditTargetDate}
                onEditPriority={setEditPriority}
                onStartEdit={() => startEditGoal(goal)}
                onSaveEdit={() => void saveEditGoal(goal.id)}
                onCancelEdit={cancelEditGoal}
                onReopen={() => void reopenGoal(goal)}
                onArchive={() => void archiveGoal(goal)}
                onDelete={() => void deleteGoal(goal)}
              />
            ))}
          </div>
        </div>
      )}

      {/* Archived Goals */}
      {archivedGoals.length > 0 && (
        <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm opacity-75">
          <h2 className="text-lg font-semibold text-gray-500 mb-4">Archived Goals ({archivedGoals.length})</h2>
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
            {archivedGoals.map((goal) => (
              <GoalCard
                key={goal.id}
                goal={goal}
                isEditing={false}
                editName=""
                editTargetAmount=""
                editTargetDate=""
                editPriority="Normal"
                saving={saving}
                onEditName={() => {}}
                onEditTargetAmount={() => {}}
                onEditTargetDate={() => {}}
                onEditPriority={() => {}}
                onStartEdit={() => {}}
                onSaveEdit={() => {}}
                onCancelEdit={() => {}}
                onReopen={() => void reopenGoal(goal)}
                onDelete={() => void deleteGoal(goal)}
              />
            ))}
          </div>
        </div>
      )}
    </div>
  );
}

interface GoalCardProps {
  goal: SavingsGoalDto;
  isEditing: boolean;
  editName: string;
  editTargetAmount: string;
  editTargetDate: string;
  editPriority: string;
  saving: boolean;
  onEditName: (v: string) => void;
  onEditTargetAmount: (v: string) => void;
  onEditTargetDate: (v: string) => void;
  onEditPriority: (v: string) => void;
  onStartEdit: () => void;
  onSaveEdit: () => void;
  onCancelEdit: () => void;
  onAddContribution?: () => void;
  onReopen?: () => void;
  onArchive?: () => void;
  onDelete: () => void;
}

function GoalCard({
  goal,
  isEditing,
  editName,
  editTargetAmount,
  editTargetDate,
  editPriority,
  saving,
  onEditName,
  onEditTargetAmount,
  onEditTargetDate,
  onEditPriority,
  onStartEdit,
  onSaveEdit,
  onCancelEdit,
  onAddContribution,
  onReopen,
  onArchive,
  onDelete,
}: GoalCardProps) {
  const isCompleted = goal.status === "Completed";
  const isArchived = goal.status === "Archived";
  const pct = Math.min(100, Number(goal.percentageComplete));
  const remaining = Math.max(0, Number(goal.remaining));

  const priorityColor =
    goal.priority === "High"
      ? "text-red-600 bg-red-50 border-red-200"
      : goal.priority === "Low"
        ? "text-gray-500 bg-gray-50 border-gray-200"
        : "text-blue-600 bg-blue-50 border-blue-200";

  const cardClass = isCompleted
    ? "p-4 bg-green-50 rounded-xl border border-green-200"
    : isArchived
      ? "p-4 bg-gray-50 rounded-xl border border-gray-200"
      : "p-4 bg-gray-50 rounded-xl border border-gray-100";

  return (
    <div className={cardClass}>
      {/* Header */}
      <div className="flex items-start justify-between mb-3 gap-2">
        <div className="flex-1 min-w-0">
          <div className="flex items-center gap-2 flex-wrap">
            <h3 className="font-semibold text-gray-900 truncate">{goal.name}</h3>
            {isCompleted && (
              <span className="inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-700 border border-green-200">
                <CheckCircle2 className="w-3 h-3" />
                Completed
              </span>
            )}
            {isArchived && (
              <span className="px-2 py-0.5 rounded-full text-xs font-medium bg-gray-200 text-gray-600 border border-gray-300">
                Archived
              </span>
            )}
          </div>
          <div className="flex items-center gap-2 mt-1 flex-wrap">
            <span className={`inline-block px-2 py-0.5 rounded-full text-xs font-medium border ${priorityColor}`}>
              {goal.priority}
            </span>
            {goal.targetDate && (
              <span className="text-xs text-gray-500">
                Target: {new Date(goal.targetDate).toLocaleDateString("en-US", { month: "short", day: "numeric", year: "numeric" })}
              </span>
            )}
          </div>
        </div>
      </div>

      {/* Progress */}
      <div className="mb-3">
        <div className="flex items-center justify-between text-sm mb-1">
          <span className="text-gray-700 font-medium">${Number(goal.currentAmount).toFixed(2)}</span>
          <span className="text-gray-500">of ${Number(goal.targetAmount).toFixed(2)}</span>
        </div>
        <div className="w-full h-2.5 rounded-full bg-gray-200 overflow-hidden">
          <div
            className={`h-full rounded-full transition-all ${isCompleted ? "bg-gradient-to-r from-green-400 to-emerald-500" : "bg-gradient-to-r from-blue-500 to-purple-600"}`}
            style={{ width: `${pct}%` }}
          />
        </div>
        <div className="flex items-center justify-between mt-1">
          <span className={`text-xs font-medium ${isCompleted ? "text-green-600" : "text-gray-600"}`}>
            {pct.toFixed(0)}% complete
          </span>
          {isCompleted ? (
            <span className="text-xs text-green-600 font-medium">Goal achieved!</span>
          ) : (
            <span className="text-xs text-gray-500">${remaining.toFixed(2)} remaining</span>
          )}
        </div>
        {isCompleted && goal.completedDate && (
          <p className="mt-1 text-xs text-green-600">
            Completed on {new Date(goal.completedDate).toLocaleDateString("en-US", { month: "short", day: "numeric", year: "numeric" })}
          </p>
        )}
      </div>

      {/* Edit form */}
      {isEditing && (
        <div className="mt-3 pt-3 border-t border-gray-200 space-y-2">
          <input
            value={editName}
            onChange={(e) => onEditName(e.target.value)}
            placeholder="Goal name"
            className="w-full px-3 py-2 border border-gray-200 rounded-xl text-sm"
          />
          <div className="grid grid-cols-2 gap-2">
            <input
              type="number"
              min="0"
              step="0.01"
              value={editTargetAmount}
              onChange={(e) => onEditTargetAmount(e.target.value)}
              placeholder="Target amount"
              className="px-3 py-2 border border-gray-200 rounded-xl text-sm"
            />
            <input
              type="date"
              value={editTargetDate}
              onChange={(e) => onEditTargetDate(e.target.value)}
              className="px-3 py-2 border border-gray-200 rounded-xl text-sm"
            />
          </div>
          <select
            value={editPriority}
            onChange={(e) => onEditPriority(e.target.value)}
            className="w-full px-3 py-2 border border-gray-200 rounded-xl text-sm"
          >
            <option value="High">High</option>
            <option value="Normal">Normal</option>
            <option value="Low">Low</option>
          </select>
          <div className="flex gap-2">
            <button
              onClick={onSaveEdit}
              disabled={saving}
              className="flex-1 inline-flex items-center justify-center gap-2 px-3 py-2 bg-blue-600 text-white rounded-xl text-sm disabled:opacity-60"
            >
              <Save className="w-4 h-4" />
              Save Changes
            </button>
            <button
              onClick={onCancelEdit}
              className="inline-flex items-center justify-center gap-2 px-3 py-2 border border-gray-200 text-gray-700 rounded-xl text-sm"
            >
              <X className="w-4 h-4" />
              Cancel
            </button>
          </div>
        </div>
      )}

      {/* Actions */}
      {!isEditing && (
        <div className="flex flex-wrap gap-2 mt-3 pt-3 border-t border-gray-200">
          {!isCompleted && !isArchived && onAddContribution && (
            <button
              onClick={onAddContribution}
              className="px-3 py-1.5 bg-blue-600 text-white rounded-lg text-sm hover:bg-blue-700"
            >
              Add Contribution
            </button>
          )}
          {!isArchived && (
            <button
              onClick={onStartEdit}
              className="inline-flex items-center gap-1.5 px-3 py-1.5 border border-gray-200 text-gray-700 rounded-lg text-sm hover:bg-gray-100"
            >
              <Pencil className="w-3.5 h-3.5" />
              Edit Goal
            </button>
          )}
          {onReopen && (
            <button
              onClick={onReopen}
              disabled={saving}
              className="inline-flex items-center gap-1.5 px-3 py-1.5 border border-blue-200 text-blue-600 rounded-lg text-sm hover:bg-blue-50 disabled:opacity-60"
            >
              <RotateCcw className="w-3.5 h-3.5" />
              Reopen
            </button>
          )}
          {onArchive && !isArchived && (
            <button
              onClick={onArchive}
              disabled={saving}
              className="inline-flex items-center gap-1.5 px-3 py-1.5 border border-gray-200 text-gray-600 rounded-lg text-sm hover:bg-gray-100 disabled:opacity-60"
            >
              <Archive className="w-3.5 h-3.5" />
              Archive
            </button>
          )}
          <button
            onClick={onDelete}
            disabled={saving}
            className="inline-flex items-center gap-1.5 px-3 py-1.5 border border-red-200 text-red-600 rounded-lg text-sm hover:bg-red-50 disabled:opacity-60"
          >
            <Trash2 className="w-3.5 h-3.5" />
            Delete
          </button>
        </div>
      )}
    </div>
  );
}

function SummaryCard({ icon, label, value }: { icon: React.ReactNode; label: string; value: string }) {
  return (
    <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
      <div className="flex items-center gap-3 mb-2">
        <div className="w-10 h-10 bg-gray-100 rounded-xl flex items-center justify-center">{icon}</div>
        <p className="text-gray-600 text-sm">{label}</p>
      </div>
      <p className="text-2xl font-bold text-gray-900">{value}</p>
    </div>
  );
}
