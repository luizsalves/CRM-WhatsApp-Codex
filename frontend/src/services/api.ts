import axios, { AxiosError } from 'axios'

const TOKEN_KEY = 'whatscrm.token'

export const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? '/api',
  timeout: 15_000,
})

api.interceptors.request.use((config) => {
  const token = localStorage.getItem(TOKEN_KEY)
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

export function salvarToken(token: string) {
  localStorage.setItem(TOKEN_KEY, token)
}

export function limparToken() {
  localStorage.removeItem(TOKEN_KEY)
}

export function possuiToken() {
  return Boolean(localStorage.getItem(TOKEN_KEY))
}

export function mensagemDeErro(error: unknown): string {
  if (error instanceof AxiosError) {
    const data = error.response?.data as { detail?: string; title?: string; errors?: Record<string, string[]> } | undefined
    const validationMessage = data?.errors ? Object.values(data.errors).flat().join(' ') : undefined
    return validationMessage ?? data?.detail ?? data?.title ?? 'Não foi possível falar com o servidor.'
  }

  return 'Ocorreu um erro inesperado.'
}
