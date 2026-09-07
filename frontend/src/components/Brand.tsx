interface BrandProps {
  compact?: boolean
}

export function Brand({ compact = false }: BrandProps) {
  return (
    <div className={`brand ${compact ? 'brand--compact' : ''}`} aria-label="WhatsCRM">
      <span className="brand__mark" aria-hidden="true">W</span>
      {!compact && (
        <span className="brand__text">
          <strong>WhatsCRM</strong>
          <small>relacionamento que vende</small>
        </span>
      )}
    </div>
  )
}
