import { Outlet } from 'react-router-dom'
import { Brand } from '../components/Brand'

export function AuthLayout() {
  return (
    <main className="auth-shell">
      <section className="auth-shell__story" aria-label="Apresentação">
        <Brand />
        <div className="auth-shell__message">
          <span className="eyebrow">Seu relacionamento, organizado</span>
          <h1>Do primeiro contato ao pós-venda.</h1>
          <p>Uma base única para sua equipe atender, acompanhar oportunidades e cuidar de cada cliente.</p>
        </div>
        <p className="auth-shell__footnote">Fundação segura, multiempresa e pronta para evoluir.</p>
      </section>
      <section className="auth-shell__content">
        <Outlet />
      </section>
    </main>
  )
}
