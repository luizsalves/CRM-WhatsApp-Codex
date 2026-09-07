import { NavLink, Outlet } from 'react-router-dom'
import { Brand } from '../components/Brand'
import { useAuth } from '../contexts/AuthContext'

const proximosModulos = ['Conversas', 'Contatos', 'CRM', 'Agenda', 'Pós-venda', 'Tarefas']

export function AppLayout() {
  const { usuario, logout } = useAuth()

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <Brand />
        <nav className="sidebar__nav" aria-label="Navegação principal">
          <NavLink to="/" end className={({ isActive }) => `nav-item ${isActive ? 'nav-item--active' : ''}`}>
            <span className="nav-item__icon" aria-hidden="true">⌂</span>
            Visão geral
          </NavLink>
          <p className="sidebar__label">Próximas etapas</p>
          {proximosModulos.map((modulo) => (
            <span className="nav-item nav-item--disabled" key={modulo} aria-disabled="true">
              <span className="nav-item__dot" aria-hidden="true" />
              {modulo}
            </span>
          ))}
        </nav>
        <div className="sidebar__profile">
          <span className="avatar" aria-hidden="true">{usuario?.nome.charAt(0).toUpperCase()}</span>
          <span className="sidebar__profile-copy">
            <strong>{usuario?.nome}</strong>
            <small>{usuario?.perfil}</small>
          </span>
          <button className="icon-button" onClick={logout} aria-label="Sair">↗</button>
        </div>
      </aside>

      <div className="app-content">
        <header className="mobile-header">
          <Brand compact />
          <div>
            <strong>{usuario?.empresaNome}</strong>
            <button onClick={logout}>Sair</button>
          </div>
        </header>
        <Outlet />
      </div>

      <nav className="bottom-nav" aria-label="Navegação móvel">
        <NavLink to="/" end>Início</NavLink>
        <span aria-disabled="true">Contatos</span>
        <span aria-disabled="true">CRM</span>
        <span aria-disabled="true">Agenda</span>
      </nav>
    </div>
  )
}
