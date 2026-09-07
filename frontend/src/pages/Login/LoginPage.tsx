import { useEffect, useState, type FormEvent } from 'react'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import { useAuth } from '../../contexts/AuthContext'
import { authService } from '../../services/authService'
import { mensagemDeErro } from '../../services/api'

export function LoginPage() {
  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [erro, setErro] = useState('')
  const [enviando, setEnviando] = useState(false)
  const [bootstrapDisponivel, setBootstrapDisponivel] = useState(false)
  const { login } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()

  useEffect(() => {
    authService.bootstrapStatus().then(({ disponivel }) => setBootstrapDisponivel(disponivel)).catch(() => undefined)
  }, [])

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setErro('')
    setEnviando(true)

    try {
      await login({ email, senha })
      const destination = (location.state as { from?: string } | null)?.from ?? '/'
      navigate(destination, { replace: true })
    } catch (error) {
      setErro(mensagemDeErro(error))
    } finally {
      setEnviando(false)
    }
  }

  return (
    <div className="auth-card">
      <span className="eyebrow">Bem-vindo de volta</span>
      <h2>Entre na sua conta</h2>
      <p className="muted">Use seus dados de acesso para continuar.</p>

      {erro && <div className="alert alert--error" role="alert">{erro}</div>}

      <form onSubmit={handleSubmit} className="form-stack">
        <label>
          E-mail
          <input
            autoComplete="email"
            inputMode="email"
            type="email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            placeholder="voce@empresa.com"
            required
          />
        </label>
        <label>
          Senha
          <input
            autoComplete="current-password"
            type="password"
            value={senha}
            onChange={(event) => setSenha(event.target.value)}
            placeholder="Sua senha"
            required
          />
        </label>
        <button className="button button--primary" disabled={enviando} type="submit">
          {enviando ? 'Entrando...' : 'Entrar'}
        </button>
      </form>

      {bootstrapDisponivel && (
        <p className="auth-card__footer">
          Primeira vez por aqui? <Link to="/primeiro-acesso">Configurar empresa</Link>
        </p>
      )}
    </div>
  )
}
