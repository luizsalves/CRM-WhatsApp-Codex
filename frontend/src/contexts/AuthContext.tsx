import { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react'
import { authService } from '../services/authService'
import { limparToken, possuiToken, salvarToken } from '../services/api'
import type { BootstrapRequest, LoginRequest, Usuario } from '../types/auth'

interface AuthContextValue {
  usuario: Usuario | null
  carregando: boolean
  login: (request: LoginRequest) => Promise<void>
  bootstrap: (request: BootstrapRequest) => Promise<void>
  logout: () => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [usuario, setUsuario] = useState<Usuario | null>(null)
  const [carregando, setCarregando] = useState(true)

  useEffect(() => {
    if (!possuiToken()) {
      setCarregando(false)
      return
    }

    authService
      .me()
      .then(setUsuario)
      .catch(() => limparToken())
      .finally(() => setCarregando(false))
  }, [])

  const login = useCallback(async (request: LoginRequest) => {
    const response = await authService.login(request)
    salvarToken(response.token)
    setUsuario(response.usuario)
  }, [])

  const bootstrap = useCallback(async (request: BootstrapRequest) => {
    const response = await authService.bootstrap(request)
    salvarToken(response.token)
    setUsuario(response.usuario)
  }, [])

  const logout = useCallback(() => {
    limparToken()
    setUsuario(null)
  }, [])

  const value = useMemo(
    () => ({ usuario, carregando, login, bootstrap, logout }),
    [usuario, carregando, login, bootstrap, logout],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

// O provider e seu hook formam uma única unidade pública de autenticação.
// eslint-disable-next-line react-refresh/only-export-components
export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth deve ser usado dentro de AuthProvider.')
  }
  return context
}
