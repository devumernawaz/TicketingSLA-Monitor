"use client";

import { useRouter } from "next/navigation";
import { useAuth, MOCK_USERS, UserRole } from "@/lib/auth";

export default function LoginPage() {
  const { login } = useAuth();
  const router = useRouter();

  const handleLogin = (role: UserRole) => {
    login(MOCK_USERS[role]);
    router.push(`/${role.toLowerCase()}`);
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-slate-50">
      <div className="bg-white p-8 rounded-lg shadow-md space-y-4">
        <h1 className="text-xl font-semibold">Mock Login — choose a role</h1>
        <div className="flex gap-3">
          <button onClick={() => handleLogin("Admin")} className="px-4 py-2 bg-slate-900 text-white rounded">
            Login as Admin
          </button>
          <button onClick={() => handleLogin("Agent")} className="px-4 py-2 bg-slate-700 text-white rounded">
            Login as Agent
          </button>
          <button onClick={() => handleLogin("Client")} className="px-4 py-2 bg-slate-500 text-white rounded">
            Login as Client
          </button>
        </div>
      </div>
    </div>
  );
}