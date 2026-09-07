import { api } from './api'
import type {
  AuthResponse,
  BootstrapRequest,
  BootstrapStatus,
  LoginRequest,
  Usuario,
} from '../types/auth'

export const authService = {
  async bootstrapStatus() {
    const { data } = await api.get<BootstrapStatus>('/auth/bootstrap-status')
    return data
  },

  async bootstrap(request: BootstrapRequest) {
    const { data } = await api.post<AuthResponse>('/auth/bootstrap', request)
    return data
  },

  async login(request: LoginRequest) {
    const { data } = await api.post<AuthResponse>('/auth/login', request)
    return data
  },

  async me() {
    const { data } = await api.get<Usuario>('/auth/me')
    return data
  },
}
