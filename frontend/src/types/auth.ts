export type TipoNegocio = 'Vendas' | 'Clinica' | 'Servicos' | 'Estetica' | 'Outro'
export type PerfilUsuario = 'Administrador' | 'Gestor' | 'Profissional' | 'Atendente'

export interface Usuario {
  id: string
  empresaId: string
  empresaNome: string
  tipoNegocio: TipoNegocio
  nome: string
  email: string
  perfil: PerfilUsuario
}

export interface AuthResponse {
  token: string
  expiraEm: string
  usuario: Usuario
}

export interface LoginRequest {
  email: string
  senha: string
}

export interface BootstrapRequest extends LoginRequest {
  empresaNome: string
  nome: string
  tipoNegocio: TipoNegocio
}

export interface BootstrapStatus {
  disponivel: boolean
}
