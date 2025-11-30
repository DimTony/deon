"use client";

import { signIn } from "next-auth/react";
import { useRouter } from "next/navigation";
import { useState, FormEvent } from "react";

export default function LoginPage() {
  const router = useRouter();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError("");
    setIsLoading(true);

    try {
      const result = await signIn("credentials", {
        email,
        password,
        redirect: false,
      });

      if (result?.error) {
        setError("Invalid email or password");
        setIsLoading(false);
        return;
      }

      router.push("/dashboard");
      router.refresh();
    } catch (error) {
      setError("An error occurred. Please try again.");
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 flex items-center justify-center p-4 relative overflow-hidden">
      {/* Animated background elements */}
      <div className="absolute inset-0 overflow-hidden">
        <div className="absolute -top-40 -right-40 w-80 h-80 bg-amber-500/10 rounded-full blur-3xl animate-pulse" />
        <div className="absolute -bottom-40 -left-40 w-80 h-80 bg-amber-600/10 rounded-full blur-3xl animate-pulse delay-1000" />
        <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-96 h-96 bg-slate-700/20 rounded-full blur-3xl" />
      </div>

      {/* Grain texture overlay */}
      <div 
        className="absolute inset-0 opacity-[0.015]"
        style={{
          backgroundImage: `url("data:image/svg+xml,%3Csvg viewBox='0 0 400 400' xmlns='http://www.w3.org/2000/svg'%3E%3Cfilter id='noiseFilter'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.9' numOctaves='4' /%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noiseFilter)' /%3E%3C/svg%3E")`,
        }}
      />

      <div className="w-full max-w-md relative z-10 animate-in fade-in slide-in-from-bottom-4 duration-1000">
        {/* Logo/Header */}
        <div className="text-center mb-8 animate-in fade-in slide-in-from-bottom-2 duration-700">
          <div className="inline-block mb-4">
            <div className="w-16 h-16 mx-auto bg-gradient-to-br from-amber-400 to-amber-600 rounded-2xl rotate-12 shadow-2xl shadow-amber-500/20 flex items-center justify-center transform hover:rotate-0 hover:scale-110 transition-all duration-500">
              <svg
                className="w-8 h-8 text-slate-900"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"
                />
              </svg>
            </div>
          </div>
          <h1 className="text-4xl font-bold text-white mb-2 tracking-tight">
            Welcome back
          </h1>
          <p className="text-slate-400 text-sm font-light tracking-wide">
            Sign in to your account to continue
          </p>
        </div>

        {/* Login Card */}
        <div className="bg-white/5 backdrop-blur-xl rounded-3xl shadow-2xl border border-white/10 p-8 animate-in fade-in slide-in-from-bottom-3 duration-900">
          <form onSubmit={handleSubmit} className="space-y-6">
            {/* Email Field */}
            <div className="space-y-2 animate-in fade-in slide-in-from-bottom-4 duration-1000">
              <label
                htmlFor="email"
                className="block text-sm font-medium text-slate-200 tracking-wide"
              >
                Email address
              </label>
              <div className="relative group">
                <div className="absolute inset-0 bg-gradient-to-r from-amber-400/20 to-amber-600/20 rounded-xl blur opacity-0 group-hover:opacity-100 transition-opacity duration-500" />
                <input
                  id="email"
                  type="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                  className="relative w-full px-4 py-3.5 bg-slate-800/50 border border-slate-700/50 rounded-xl text-white placeholder-slate-500 focus:outline-none focus:border-amber-500/50 focus:ring-2 focus:ring-amber-500/20 transition-all duration-300"
                  placeholder="you@example.com"
                  disabled={isLoading}
                />
              </div>
            </div>

            {/* Password Field */}
            <div className="space-y-2 animate-in fade-in slide-in-from-bottom-5 duration-1100">
              <div className="flex items-center justify-between">
                <label
                  htmlFor="password"
                  className="block text-sm font-medium text-slate-200 tracking-wide"
                >
                  Password
                </label>
                <button
                  type="button"
                  className="text-sm text-amber-400 hover:text-amber-300 transition-colors duration-200 font-medium"
                >
                  Forgot?
                </button>
              </div>
              <div className="relative group">
                <div className="absolute inset-0 bg-gradient-to-r from-amber-400/20 to-amber-600/20 rounded-xl blur opacity-0 group-hover:opacity-100 transition-opacity duration-500" />
                <input
                  id="password"
                  type="password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                  className="relative w-full px-4 py-3.5 bg-slate-800/50 border border-slate-700/50 rounded-xl text-white placeholder-slate-500 focus:outline-none focus:border-amber-500/50 focus:ring-2 focus:ring-amber-500/20 transition-all duration-300"
                  placeholder="••••••••"
                  disabled={isLoading}
                />
              </div>
            </div>

            {/* Error Message */}
            {error && (
              <div className="bg-red-500/10 border border-red-500/20 text-red-400 px-4 py-3 rounded-xl text-sm animate-in fade-in slide-in-from-top-2 duration-300">
                {error}
              </div>
            )}

            {/* Submit Button */}
            <button
              type="submit"
              disabled={isLoading}
              className="relative w-full group animate-in fade-in slide-in-from-bottom-6 duration-1200"
            >
              <div className="absolute inset-0 bg-gradient-to-r from-amber-400 to-amber-600 rounded-xl blur group-hover:blur-md transition-all duration-300" />
              <div className="relative px-4 py-3.5 bg-gradient-to-r from-amber-400 to-amber-600 rounded-xl font-semibold text-slate-900 hover:shadow-2xl hover:shadow-amber-500/30 transform hover:-translate-y-0.5 transition-all duration-300 flex items-center justify-center">
                {isLoading ? (
                  <>
                    <svg
                      className="animate-spin -ml-1 mr-3 h-5 w-5 text-slate-900"
                      xmlns="http://www.w3.org/2000/svg"
                      fill="none"
                      viewBox="0 0 24 24"
                    >
                      <circle
                        className="opacity-25"
                        cx="12"
                        cy="12"
                        r="10"
                        stroke="currentColor"
                        strokeWidth="4"
                      />
                      <path
                        className="opacity-75"
                        fill="currentColor"
                        d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                      />
                    </svg>
                    Signing in...
                  </>
                ) : (
                  <>
                    Sign in
                    <svg
                      className="w-5 h-5 ml-2 group-hover:translate-x-1 transition-transform duration-300"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="M13 7l5 5m0 0l-5 5m5-5H6"
                      />
                    </svg>
                  </>
                )}
              </div>
            </button>

            {/* Demo Credentials */}
            <div className="pt-4 border-t border-slate-700/50 animate-in fade-in duration-1300">
              <p className="text-xs text-slate-400 text-center">
                Demo credentials:{" "}
                <span className="text-amber-400 font-mono">demo@example.com</span> /{" "}
                <span className="text-amber-400 font-mono">password123</span>
              </p>
            </div>
          </form>
        </div>

        {/* Footer */}
        <p className="mt-8 text-center text-sm text-slate-400 animate-in fade-in duration-1400">
          Don't have an account?{" "}
          <button className="text-amber-400 hover:text-amber-300 font-medium transition-colors duration-200">
            Sign up
          </button>
        </p>
      </div>
    </div>
  );
}
