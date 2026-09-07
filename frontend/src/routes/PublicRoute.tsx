import { Navigate, Outlet } from 'react-router-dom'
import { LoadingScreen } from '../components/LoadingScreen'
import { useAuth } from '../contexts/AuthContext'

export function PublicRoute() {
  const { usuario, carregando } = useAuth()

  if (carregando) {
    return <LoadingScreen />
  }

  return usuario ? <Navigate to="/" replace /> : <Outlet />
}
