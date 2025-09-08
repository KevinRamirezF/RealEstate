import {themes as prismThemes} from 'prism-react-renderer';
import type {Config} from '@docusaurus/types';
import type * as Preset from '@docusaurus/preset-classic';

const config: Config = {
  title: 'RealEstate API Documentation',
  tagline: 'Comprehensive documentation for the RealEstate .NET application',
  favicon: 'img/favicon.ico',

  // Set the production url of your site here
  url: 'https://your-docusaurus-site.example.com',
  // Set the /<baseUrl>/ pathname under which your site is served
  baseUrl: '/',

  // GitHub pages deployment config.
  organizationName: 'realestate-team',
  projectName: 'realestate-docs',

  onBrokenLinks: 'warn',
  onBrokenMarkdownLinks: 'warn',

  // Even if you don't use internationalization, you can use this field to set
  // useful metadata like html lang.
  i18n: {
    defaultLocale: 'en',
    locales: ['en'],
  },

  presets: [
    [
      'classic',
      {
        docs: {
          sidebarPath: './sidebars.ts',
          editUrl: 'https://github.com/your-org/realestate/tree/main/docs/',
        },
        theme: {
          customCss: './src/css/custom.css',
        },
      } satisfies Preset.Options,
    ],
  ],

  themeConfig: {
    image: 'img/docusaurus-social-card.jpg',
    navbar: {
      title: 'RealEstate Docs',
      logo: {
        alt: 'RealEstate Logo',
        src: 'img/logo.svg',
      },
      items: [
        {
          type: 'docSidebar',
          sidebarId: 'tutorialSidebar',
          position: 'left',
          label: 'Documentation',
        },
        {
          to: '/docs/architecture/overview',
          label: 'Architecture',
          position: 'left',
        },
        {
          to: '/docs/architecture/diagrams/class-diagram-domain-entities',
          label: 'Diagrams',
          position: 'left',
        },
        {
          to: '/docs/api-reference',
          label: 'API Reference',
          position: 'left',
        },
        {
          to: '/docs/adrs',
          label: 'ADRs',
          position: 'left',
        },
        {
          to: '/docs/development/test-coverage',
          label: 'Cobertura',
          position: 'left',
        },
        {
          href: 'https://github.com/your-org/realestate',
          label: 'GitHub',
          position: 'right',
        },
      ],
    },
    footer: {
      style: 'dark',
      links: [
        {
          title: 'Docs',
          items: [
            {
              label: 'Getting Started',
              to: '/docs/intro',
            },
            {
              label: 'Architecture',
              to: '/docs/architecture/overview',
            },
            {
              label: 'Diagrams',
              to: '/docs/architecture/diagrams/class-diagram-domain-entities',
            },
          ],
        },
        {
          title: 'Development',
          items: [
            {
              label: 'ADRs',
              to: '/docs/adrs',
            },
          ],
        },
        {
          title: 'More',
          items: [
            {
              label: 'GitHub',
              href: 'https://github.com/your-org/realestate',
            },
          ],
        },
      ],
      copyright: `Copyright © ${new Date().getFullYear()} RealEstate Team. Built with Docusaurus.`,
    },
    prism: {
      theme: prismThemes.github,
      darkTheme: prismThemes.dracula,
      additionalLanguages: ['csharp', 'json', 'sql'],
    },
  } satisfies Preset.ThemeConfig,

  plugins: [
    // Add plugins here for Mermaid and other features
  ],

  markdown: {
    mermaid: true,
  },
  themes: ['@docusaurus/theme-mermaid'],
};

export default config;