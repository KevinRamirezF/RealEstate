---
id: test-coverage
title: Cobertura de Pruebas
description: Resultados de cobertura de pruebas automatizadas
---

import useBaseUrl from '@docusaurus/useBaseUrl';

# Cobertura de Pruebas

A continuación se muestran los resultados de cobertura de pruebas automatizadas del proyecto. Estos informes se generan automáticamente durante el proceso de integración continua.

## Resumen de Cobertura

<div className="coverage-summary">
  <div className="coverage-card">
    <h3>Cobertura Total</h3>
    <div className="coverage-value" id="total-coverage">Cargando...</div>
  </div>
  <div className="coverage-card">
    <h3>Líneas Cubiertas</h3>
    <div className="coverage-value" id="line-coverage">Cargando...</div>
  </div>
  <div className="coverage-card">
    <h3>Ramas Cubiertas</h3>
    <div className="coverage-value" id="branch-coverage">Cargando...</div>
  </div>
</div>

## Informe Detallado

Puedes ver el informe completo de cobertura haciendo clic en el siguiente botón:

<a href={useBaseUrl('/coverage/index.html')} className="button button--primary" target="_blank">Ver Informe Completo</a>

:::note
El informe se abre en una nueva pestaña y muestra información detallada sobre qué partes del código están cubiertas por pruebas.
:::

## Cómo se Genera

1. Las pruebas se ejecutan automáticamente en cada confirmación
2. Se genera un informe de cobertura usando `dotnet test` con `Coverlet`
3. El informe se procesa y se publica automáticamente en la documentación

## Mejorando la Cobertura

- Escribe pruebas para los casos de borde
- Asegúrate de probar todas las ramas condicionales
- Mantén las pruebas actualizadas con los cambios en el código

<style>{`
.coverage-summary {
  display: flex;
  justify-content: space-around;
  margin: 2rem 0;
  flex-wrap: wrap;
}

.coverage-card {
  background: var(--ifm-card-background-color);
  border-radius: var(--ifm-card-border-radius);
  box-shadow: var(--ifm-card-shadow);
  padding: 1.5rem;
  text-align: center;
  min-width: 200px;
  margin: 0.5rem;
}

.coverage-card h3 {
  margin-top: 0;
  color: var(--ifm-color-primary);
}

.coverage-value {
  font-size: 2rem;
  font-weight: bold;
  margin: 0.5rem 0;
}

@media (max-width: 768px) {
  .coverage-summary {
    flex-direction: column;
    align-items: center;
  }
  
  .coverage-card {
    width: 80%;
    margin-bottom: 1rem;
  }
}
`}</style>

<script dangerouslySetInnerHTML={{
  __html: `
  document.addEventListener('DOMContentLoaded', function() {
    fetch('/coverage/coverage-summary.json')
      .then(response => response.json())
      .then(data => {
        const total = data.total;
        document.getElementById('total-coverage').textContent = total.lines.pct + '%';
        document.getElementById('line-coverage').textContent = total.lines.covered + ' de ' + total.lines.total;
        document.getElementById('branch-coverage').textContent = total.branches.pct + '%';
      })
      .catch(error => {
        console.error('Error al cargar los datos de cobertura:', error);
        document.getElementById('total-coverage').textContent = 'No disponible';
        document.getElementById('line-coverage').textContent = 'No disponible';
        document.getElementById('branch-coverage').textContent = 'No disponible';
      });
  });
  `
}}></script>
