import React from 'react';
import ComponentCreator from '@docusaurus/ComponentCreator';

export default [
  {
    path: '/__docusaurus/debug',
    component: ComponentCreator('/__docusaurus/debug', '5ff'),
    exact: true
  },
  {
    path: '/__docusaurus/debug/config',
    component: ComponentCreator('/__docusaurus/debug/config', '5ba'),
    exact: true
  },
  {
    path: '/__docusaurus/debug/content',
    component: ComponentCreator('/__docusaurus/debug/content', 'a2b'),
    exact: true
  },
  {
    path: '/__docusaurus/debug/globalData',
    component: ComponentCreator('/__docusaurus/debug/globalData', 'c3c'),
    exact: true
  },
  {
    path: '/__docusaurus/debug/metadata',
    component: ComponentCreator('/__docusaurus/debug/metadata', '156'),
    exact: true
  },
  {
    path: '/__docusaurus/debug/registry',
    component: ComponentCreator('/__docusaurus/debug/registry', '88c'),
    exact: true
  },
  {
    path: '/__docusaurus/debug/routes',
    component: ComponentCreator('/__docusaurus/debug/routes', '000'),
    exact: true
  },
  {
    path: '/adrs/',
    component: ComponentCreator('/adrs/', 'ed4'),
    exact: true
  },
  {
    path: '/api-reference/',
    component: ComponentCreator('/api-reference/', '4fe'),
    exact: true
  },
  {
    path: '/architecture/',
    component: ComponentCreator('/architecture/', 'b84'),
    exact: true
  },
  {
    path: '/database/',
    component: ComponentCreator('/database/', '99c'),
    exact: true
  },
  {
    path: '/docs',
    component: ComponentCreator('/docs', '1c1'),
    routes: [
      {
        path: '/docs',
        component: ComponentCreator('/docs', '5cf'),
        routes: [
          {
            path: '/docs',
            component: ComponentCreator('/docs', '518'),
            routes: [
              {
                path: '/docs/adrs/',
                component: ComponentCreator('/docs/adrs/', 'b8d'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/docs/adrs/clean-architecture-adoption',
                component: ComponentCreator('/docs/adrs/clean-architecture-adoption', '873'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/docs/adrs/cqrs-light-implementation',
                component: ComponentCreator('/docs/adrs/cqrs-light-implementation', '964'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/docs/adrs/domain-driven-design-patterns',
                component: ComponentCreator('/docs/adrs/domain-driven-design-patterns', '1de'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/docs/adrs/ef-core-repository-pattern',
                component: ComponentCreator('/docs/adrs/ef-core-repository-pattern', 'f72'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/docs/adrs/fluentvalidation-input-validation',
                component: ComponentCreator('/docs/adrs/fluentvalidation-input-validation', 'eb7'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/docs/adrs/soft-delete-implementation',
                component: ComponentCreator('/docs/adrs/soft-delete-implementation', 'f45'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/docs/api-reference',
                component: ComponentCreator('/docs/api-reference', '756'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/docs/architecture/diagrams/c4-architecture-diagrams',
                component: ComponentCreator('/docs/architecture/diagrams/c4-architecture-diagrams', '18f'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/docs/architecture/diagrams/class-diagram-domain-entities',
                component: ComponentCreator('/docs/architecture/diagrams/class-diagram-domain-entities', 'df9'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/docs/architecture/diagrams/entity-relationship-diagram',
                component: ComponentCreator('/docs/architecture/diagrams/entity-relationship-diagram', 'e05'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/docs/architecture/diagrams/sequence-diagrams',
                component: ComponentCreator('/docs/architecture/diagrams/sequence-diagrams', 'c39'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/docs/architecture/overview',
                component: ComponentCreator('/docs/architecture/overview', '833'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/docs/getting-started/configuration',
                component: ComponentCreator('/docs/getting-started/configuration', '468'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/docs/getting-started/installation',
                component: ComponentCreator('/docs/getting-started/installation', '267'),
                exact: true,
                sidebar: "tutorialSidebar"
              },
              {
                path: '/docs/intro',
                component: ComponentCreator('/docs/intro', '61d'),
                exact: true,
                sidebar: "tutorialSidebar"
              }
            ]
          }
        ]
      }
    ]
  },
  {
    path: '/',
    component: ComponentCreator('/', 'e5f'),
    exact: true
  },
  {
    path: '*',
    component: ComponentCreator('*'),
  },
];
