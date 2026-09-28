export function LoadingOverlay({ message = 'Loading...' }: { message?: string }) {
  return (
    <div className="flex flex-col items-center justify-center py-20">
      <div className="animate-spin h-10 w-10 border-4 border-gray-200 border-t-[var(--color-primary)] rounded-full" />
      <p className="mt-4 text-sm text-gray-500">{message}</p>
    </div>
  );
}
