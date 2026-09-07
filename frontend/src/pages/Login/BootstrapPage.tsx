import { useEffect, useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../../contexts/AuthContext'
import { authService } from '../../services/authService'
import { mensagemDeErro } from '../../services/api'
import type { TipoNegocio } from '../../types/auth'

export function BootstrapPage() {
  const [empresaNome, setEmpresaNome] = useState('')
  const [nome, setNome] = useState('')
  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [tipoNegocio, setTipoNegocio] = useState<TipoNegocio>('Vendas')
  const [erro, setErro] = useState('')
  const [enviando, setEnviando] = useState(false)
  const { bootstrap } = useAuth()
  const navigate = useNavigate()

  useEffect(() => {
    authService.bootstrapStatus()
      .then(({ disponivel }) => {
        if (!disponivel) navigate('/login', { replace: true })
      })
      .catch(() => undefined)
  }, [navigate])

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setErro('')
    setEnviando(true)

    try {
      await bootstrap({ empresaNome, nome, email, senha, tipoNegocio })
      navigate('/', { replace: true })
    } catch (error) {
      setErro(mensagemDeErro(error))
    } finally {
      setEnviando(false)
    }
  }

  return (
    <div className="auth-card auth-card--wide">
      <span className="eyebrow">Configuração inicial</span>
      <h2>Crie o primeiro acesso</h2>
      <p className="muted">Essa conta será a administradora da primeira empresa.</p>

      {erro && <div className="alert alert--error" role="alert">{erro}</div>}

      <form onSubmit={handleSubmit} className="form-stack">
        <div className="form-grid">
          <label>
            Nome da empresa
            <input value={empresaNome} onChange={(event) => setEmpresaNome(event.target.value)} required minLength={2} />
          </label>
          <label>
            Tipo de negócio
            <select value={tipoNegocio} onChange={(event) => setTipoNegocio(event.target.value as TipoNegocio)}>
              <option value="Vendas">Vendas</option>
              <option value="Clinica">Clínica</option>
              <option value="Servicos">Serviços</option>
              <option value="Estetica">Estética</option>
              <option value="Outro">Outro</option>
            </select>
          </label>
        </div>
        <label>
          Seu nome
          <input autoComplete="name" value={nome} onChange={(event) => setNome(event.target.value)} required minLength={2} />
        </label>
        <label>
          E-mail
          <input autoComplete="email" type="email" value={email} onChange={(event) => setEmail(event.target.value)} required />
        </label>
        <label>
          Senha
          <input autoComplete="new-password" type="password" value={senha} onChange={(event) => setSenha(event.target.value)} required minLength={12} />
          <small>Use 12+ caracteres, com maiúscula, minúscula e número.</small>
        </label>
        <button className="button button--primary" disabled={enviando} type="submit">
          {enviando ? 'Criando ambiente...' : 'Criar empresa e entrar'}
        </button>
      </form>
      <p className="auth-card__footer"><Link to="/login">Voltar para o login</Link></p>
    </div>
  )
}
