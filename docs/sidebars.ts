import type {SidebarsConfig} from '@docusaurus/plugin-content-docs';

const sidebars: SidebarsConfig = {
  tutorialSidebar: [
    'intro',
    {
      type: 'category',
      label: 'Getting Started',
      items: [
        'getting-started/installation',
        'getting-started/configuration',
      ],
    },
    {
      type: 'category',
      label: 'Architecture',
      items: [
        'architecture/overview',
        {
          type: 'category',
          label: 'Diagrams',
          items: [
            'architecture/diagrams/class-diagram-domain-entities',
            'architecture/diagrams/sequence-diagrams',
            'architecture/diagrams/entity-relationship-diagram',
            'architecture/diagrams/c4-architecture-diagrams',
          ],
        },
      ],
    },
    'api-reference',
    {
      type: 'category',
      label: 'Architecture Decision Records',
      items: [
        'adrs/index',
        'adrs/clean-architecture-adoption',
        'adrs/cqrs-light-implementation', 
        'adrs/ef-core-repository-pattern',
        'adrs/domain-driven-design-patterns',
        'adrs/fluentvalidation-input-validation',
        'adrs/soft-delete-implementation',
      ],
    },
  ],
};

export default sidebars;