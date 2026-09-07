import { useAuth } from '../../contexts/AuthContext'

const modulos = [
  ['Contatos e tags', 'SPEC-003'],
  ['WhatsApp e conversas', 'SPEC-004/005'],
  ['CRM e negócios', 'SPEC-006'],
  ['Agenda', 'SPEC-007'],
  ['Pós-venda', 'SPEC-008'],
  ['Tarefas', 'SPEC-009'],
]

export function DashboardPage() {
  const { usuario } = useAuth()

  return (
    <main className="dashboard">
      <header className="page-header">
        <div>
          <span className="eyebrow">Fundação ativa</span>
          <h1>Olá, {usuario?.nome.split(' ')[0]}.</h1>
          <p>Seu ambiente multiempresa está pronto para receber os próximos módulos.</p>
        </div>
        <span className="status-pill"><span /> Sistema operacional</span>
      </header>

      <section className="foundation-card">
        <div>
          <span className="foundation-card__number">01</span>
          <p>SPEC concluída</p>
        </div>
        <div className="foundation-card__copy">
          <span className="eyebrow">Base do produto</span>
          <h2>Autenticação e isolamento por empresa</h2>
          <p>JWT, usuário administrador, rotas protegidas e contexto seguro de empresa já fazem parte do núcleo.</p>
        </div>
        <dl>
          <div><dt>Empresa</dt><dd>{usuario?.empresaNome}</dd></div>
          <div><dt>Vertical</dt><dd>{usuario?.tipoNegocio}</dd></div>
          <div><dt>Perfil</dt><dd>{usuario?.perfil}</dd></div>
        </dl>
      </section>

      <section className="roadmap-section">
        <div className="section-heading">
          <div>
            <span className="eyebrow">Roadmap incremental</span>
            <h2>Próximos módulos</h2>
          </div>
          <p>Cada módulo será implementado somente após sua especificação.</p>
        </div>
        <div className="module-grid">
          {modulos.map(([nome, spec]) => (
            <article className="module-card" key={nome}>
              <span>{spec}</span>
              <h3>{nome}</h3>
              <p>Aguardando implementação</p>
            </article>
          ))}
        </div>
      </section>
    </main>
  )
}
