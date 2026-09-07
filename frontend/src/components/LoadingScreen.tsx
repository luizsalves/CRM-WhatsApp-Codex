export function LoadingScreen() {
  return (
    <main className="loading-screen" aria-live="polite" aria-label="Carregando">
      <span className="spinner" aria-hidden="true" />
      <p>Preparando seu espaço...</p>
    </main>
  )
}
