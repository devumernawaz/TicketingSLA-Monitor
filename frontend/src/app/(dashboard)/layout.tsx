export default function DashboardLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="flex min-h-screen">
      <aside className="w-64 bg-slate-900 text-white p-4">
        <p className="font-semibold">Sidebar placeholder</p>
      </aside>
      <main className="flex-1 bg-slate-50">{children}</main>
    </div>
  );
}