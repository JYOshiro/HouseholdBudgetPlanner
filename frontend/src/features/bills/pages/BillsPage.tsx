import { useEffect, useMemo, useState } from "react";
import { Calendar, CheckCircle2, Clock, AlertCircle, Plus } from "lucide-react";
import { useAuth } from "../../auth/context/AuthContext";
import { ApiError } from "../../../shared/api/httpClient";
import { financeApi, type BillDto, type CategoryDto } from "../../../shared/api/financeApi";

const frequencyOptions = ["OneTime", "Monthly", "Quarterly", "Annual"];

export default function BillsPage() {
  const { token } = useAuth();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [bills, setBills] = useState<BillDto[]>([]);
  const [expenseCategories, setExpenseCategories] = useState<CategoryDto[]>([]);
  const [showForm, setShowForm] = useState(false);
  const [saving, setSaving] = useState(false);

  const [name, setName] = useState("");
  const [amount, setAmount] = useState("");
  const [dueDate, setDueDate] = useState(new Date().toISOString().slice(0, 10));
  const [frequency, setFrequency] = useState("Monthly");
  const [categoryId, setCategoryId] = useState<number>(0);

  const loadData = async () => {
    if (!token) return;
    setLoading(true);
    setError(null);

    try {
      const [billData, categories] = await Promise.all([
        financeApi.getBills(token),
        financeApi.getCategories(token, "Expense"),
      ]);
      setBills(billData);
      setExpenseCategories(categories);
      if (categories[0] && categoryId === 0) {
        setCategoryId(categories[0].id);
      }
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to load bills.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadData();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token]);

  const today = new Date();
  const upcomingBills = useMemo(
    () => bills.filter((bill) => !bill.isPaid && new Date(bill.dueDate) >= today),
    [bills, today],
  );
  const overdueBills = useMemo(
    () => bills.filter((bill) => !bill.isPaid && new Date(bill.dueDate) < today),
    [bills, today],
  );
  const paidBills = useMemo(() => bills.filter((bill) => bill.isPaid), [bills]);

  const createBill = async () => {
    if (!token) return;
    if (!name.trim() || !amount || !dueDate || categoryId <= 0) {
      setError("Please complete all bill fields.");
      return;
    }

    setSaving(true);
    setError(null);

    try {
      await financeApi.createBill(token, {
        name: name.trim(),
        amount: Number(amount),
        dueDate: new Date(dueDate).toISOString(),
        frequency,
        categoryId,
      });
      setShowForm(false);
      setName("");
      setAmount("");
      await loadData();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to create bill.");
    } finally {
      setSaving(false);
    }
  };

  const markPaid = async (billId: number) => {
    if (!token) return;
    try {
      await financeApi.markBillAsPaid(token, billId, new Date().toISOString());
      await loadData();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to mark bill as paid.");
    }
  };

  const totalUpcoming = upcomingBills.reduce((sum, b) => sum + Number(b.amount), 0);
  const totalPaid = paidBills.reduce((sum, b) => sum + Number(b.amount), 0);
  const totalOverdue = overdueBills.reduce((sum, b) => sum + Number(b.amount), 0);

  return (
    <div className="max-w-7xl mx-auto space-y-6">
      <div className="flex items-center justify-between flex-wrap gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Bills</h1>
          <p className="text-gray-600 mt-1">Live recurring and one-time bills from your database</p>
        </div>
        <button
          onClick={() => setShowForm((v) => !v)}
          className="flex items-center gap-2 px-4 py-2 bg-gradient-to-r from-blue-500 to-purple-600 text-white rounded-xl hover:shadow-lg"
        >
          <Plus className="w-5 h-5" />
          Add Bill
        </button>
      </div>

      {showForm && (
        <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm grid grid-cols-1 md:grid-cols-6 gap-3">
          <input value={name} onChange={(e) => setName(e.target.value)} placeholder="Bill name" className="px-3 py-2 border border-gray-200 rounded-xl" />
          <input value={amount} onChange={(e) => setAmount(e.target.value)} type="number" step="0.01" min="0" placeholder="Amount" className="px-3 py-2 border border-gray-200 rounded-xl" />
          <input value={dueDate} onChange={(e) => setDueDate(e.target.value)} type="date" className="px-3 py-2 border border-gray-200 rounded-xl" />
          <select value={frequency} onChange={(e) => setFrequency(e.target.value)} className="px-3 py-2 border border-gray-200 rounded-xl">
            {frequencyOptions.map((option) => (
              <option key={option} value={option}>{option}</option>
            ))}
          </select>
          <select value={categoryId} onChange={(e) => setCategoryId(Number(e.target.value))} className="px-3 py-2 border border-gray-200 rounded-xl">
            {expenseCategories.map((c) => (
              <option key={c.id} value={c.id}>{c.name}</option>
            ))}
          </select>
          <button onClick={() => void createBill()} disabled={saving} className="px-4 py-2 bg-blue-600 text-white rounded-xl disabled:opacity-60">
            {saving ? "Saving..." : "Save"}
          </button>
        </div>
      )}

      {error && <div className="p-3 rounded-xl bg-red-50 border border-red-200 text-red-700">{error}</div>}

      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <StatCard icon={<Clock className="w-5 h-5 text-orange-600" />} title="Upcoming" value={totalUpcoming} subtitle={`${upcomingBills.length} bills`} />
        <StatCard icon={<CheckCircle2 className="w-5 h-5 text-green-600" />} title="Paid" value={totalPaid} subtitle={`${paidBills.length} bills`} />
        <StatCard icon={<AlertCircle className="w-5 h-5 text-red-600" />} title="Overdue" value={totalOverdue} subtitle={`${overdueBills.length} bills`} />
        <StatCard icon={<Calendar className="w-5 h-5 text-blue-600" />} title="Total Monthly" value={bills.reduce((sum, bill) => sum + Number(bill.amount), 0)} subtitle={`${bills.length} bills`} />
      </div>

      {loading ? (
        <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm text-gray-500">Loading bills...</div>
      ) : (
        <>
          <BillSection
            title="Overdue Bills"
            bills={overdueBills}
            tone="red"
            actionLabel="Pay Now"
            onAction={markPaid}
          />
          <BillSection
            title="Upcoming Bills"
            bills={upcomingBills}
            tone="orange"
            actionLabel="Mark Paid"
            onAction={markPaid}
          />
          <BillSection title="Paid Bills" bills={paidBills} tone="green" />
        </>
      )}
    </div>
  );
}

function StatCard({ icon, title, value, subtitle }: { icon: React.ReactNode; title: string; value: number; subtitle: string }) {
  return (
    <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
      <div className="flex items-center gap-3 mb-2">
        <div className="w-10 h-10 bg-gray-100 rounded-xl flex items-center justify-center">{icon}</div>
        <p className="text-gray-600 text-sm">{title}</p>
      </div>
      <p className="text-2xl font-bold text-gray-900">${value.toFixed(2)}</p>
      <p className="text-sm text-gray-500 mt-1">{subtitle}</p>
    </div>
  );
}

function BillSection({
  title,
  bills,
  tone,
  actionLabel,
  onAction,
}: {
  title: string;
  bills: BillDto[];
  tone: "red" | "orange" | "green";
  actionLabel?: string;
  onAction?: (billId: number) => void;
}) {
  const bg = tone === "red" ? "bg-red-50 border-red-200" : tone === "orange" ? "bg-orange-50 border-orange-200" : "bg-green-50 border-green-200";

  return (
    <div className="bg-white rounded-2xl p-6 border border-gray-200 shadow-sm">
      <h2 className="text-lg font-semibold text-gray-900 mb-4">{title}</h2>
      {bills.length === 0 ? (
        <p className="text-gray-500">No bills in this section.</p>
      ) : (
        <div className="space-y-3">
          {bills.map((bill) => (
            <div key={bill.id} className={`flex items-center justify-between p-4 rounded-xl border ${bg}`}>
              <div>
                <p className="font-semibold text-gray-900">{bill.name}</p>
                <p className="text-sm text-gray-600">
                  {bill.categoryName} • Due {new Date(bill.dueDate).toLocaleDateString("en-US", { month: "short", day: "numeric", year: "numeric" })}
                </p>
              </div>
              <div className="text-right">
                <p className="font-bold text-gray-900">${Number(bill.amount).toFixed(2)}</p>
                {actionLabel && onAction && !bill.isPaid && (
                  <button
                    onClick={() => onAction(bill.id)}
                    className="mt-2 px-3 py-1 bg-blue-600 text-white text-sm rounded-lg hover:bg-blue-700"
                  >
                    {actionLabel}
                  </button>
                )}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
