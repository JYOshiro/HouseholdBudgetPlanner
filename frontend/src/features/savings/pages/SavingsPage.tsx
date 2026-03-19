import { useEffect, useState } from "react";
import { Target, Plus, TrendingUp, Calendar, DollarSign } from "lucide-react";
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

  const [name, setName] = useState("");
  const [targetAmount, setTargetAmount] = useState("");
  const [targetDate, setTargetDate] = useState("");
  const [priority, setPriority] = useState("Normal");

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

  const totalSaved = goals.reduce((sum, goal) => sum + Number(goal.currentAmount), 0);
  const totalTarget = goals.reduce((sum, goal) => sum + Number(goal.targetAmount), 0);
  const remaining = totalTarget - totalSaved;

  return (
    <div className="max-w-7xl mx-auto space-y-6">
      <div className="flex items-center justify-between flex-wrap gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Savings Goals</h1>
          <p className="text-gray-600 mt-1">Live goals and progress from your household data</p>
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
        <SummaryCard icon={<DollarSign className="w-5 h-5 text-green-600" />} label="Total Saved" value={`$${totalSaved.toFixed(2)}`} />
        <SummaryCard icon={<Target className="w-5 h-5 text-blue-600" />} label="Total Target" value={`$${totalTarget.toFixed(2)}`} />
        <SummaryCard icon={<TrendingUp className="w-5 h-5 text-purple-600" />} label="Goals" value={`${goals.length}`} />
        <SummaryCard icon={<Calendar className="w-5 h-5 text-orange-600" />} label="Remaining" value={`$${remaining.toFixed(2)}`} />
      </div>

      <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
        <h2 className="text-lg font-semibold text-gray-900 mb-4">Goal Progress</h2>
        {loading ? (
          <p className="text-gray-500">Loading goals...</p>
        ) : goals.length === 0 ? (
          <p className="text-gray-500">No goals yet. Create your first goal.</p>
        ) : (
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
            {goals.map((goal) => (
              <div key={goal.id} className="p-4 bg-gray-50 rounded-xl border border-gray-100">
                <div className="flex items-center justify-between mb-3">
                  <div>
                    <h3 className="font-semibold text-gray-900">{goal.name}</h3>
                    <p className="text-sm text-gray-600">Priority: {goal.priority}</p>
                  </div>
                  <button
                    onClick={() => void addContribution(goal.id)}
                    className="px-3 py-1.5 bg-blue-600 text-white rounded-lg text-sm hover:bg-blue-700"
                  >
                    Add Contribution
                  </button>
                </div>

                <div className="flex items-center justify-between text-sm mb-2">
                  <span className="text-gray-700">${Number(goal.currentAmount).toFixed(2)}</span>
                  <span className="text-gray-600">of ${Number(goal.targetAmount).toFixed(2)}</span>
                </div>
                <div className="w-full h-2 rounded-full bg-gray-200 overflow-hidden">
                  <div className="h-full bg-gradient-to-r from-blue-500 to-purple-600" style={{ width: `${Math.min(100, Number(goal.percentageComplete))}%` }} />
                </div>
                <p className="mt-2 text-sm text-gray-600">
                  {Number(goal.percentageComplete).toFixed(0)}% complete, remaining ${Number(goal.remaining).toFixed(2)}
                </p>
                {goal.targetDate && (
                  <p className="mt-1 text-xs text-gray-500">
                    Target date: {new Date(goal.targetDate).toLocaleDateString("en-US", { month: "short", day: "numeric", year: "numeric" })}
                  </p>
                )}
              </div>
            ))}
          </div>
        )}
      </div>
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
