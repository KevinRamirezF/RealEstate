# C4 Diagrams Source Files

This folder contains the PlantUML source files for generating C4 architecture diagrams for the RealEstate application.

## Files Overview

1. **01-system-context.puml** - System Context Diagram (Level 1)
2. **02-container.puml** - Container Diagram (Level 2)
3. **03-api-components.puml** - API Components Diagram (Level 3)
4. **04-webapp-components.puml** - Web Application Components Diagram (Level 3)
5. **05-deployment.puml** - Deployment Diagram

## How to Generate SVG Files

### Option 1: Using PlantUML Online Server

1. Go to [PlantUML Online Server](http://www.plantuml.com/plantuml/uml/)
2. Copy the content of each `.puml` file
3. Paste it into the online editor
4. Click "Submit" to generate the diagram
5. Right-click on the generated image and save as SVG
6. Rename the files according to the pattern below

### Option 2: Using PlantUML CLI (Recommended)

#### Prerequisites
- Install Java 8 or higher
- Download PlantUML JAR from [http://plantuml.com/download](http://plantuml.com/download)

#### Generate SVG Files
```bash
# Navigate to this folder
cd docs/c4-diagrams-source

# Generate all SVG files
java -jar plantuml.jar -tsvg *.puml

# Or generate individual files
java -jar plantuml.jar -tsvg 01-system-context.puml
java -jar plantuml.jar -tsvg 02-container.puml
java -jar plantuml.jar -tsvg 03-api-components.puml
java -jar plantuml.jar -tsvg 04-webapp-components.puml
java -jar plantuml.jar -tsvg 05-deployment.puml
```

### Option 3: Using VS Code Extension

1. Install the "PlantUML" extension in VS Code
2. Open each `.puml` file
3. Press `Ctrl+Shift+P` (or `Cmd+Shift+P` on Mac)
4. Type "PlantUML: Export Current Diagram" and select it
5. Choose SVG format
6. Save to the target folder

## Target File Names

After generating the SVG files, copy them to `/docs/static/img/c4-diagrams/` with these exact names:

- `01-system-context.svg`
- `02-container.svg`
- `03-api-components.svg`
- `04-webapp-components.svg`
- `05-deployment.svg`

## Final Steps

1. Copy the generated SVG files to `/docs/static/img/c4-diagrams/`
2. Uncomment the image references in `/docs/docs/architecture/diagrams/c4-architecture-diagrams.md`
3. Rebuild the Docusaurus site: `npm run build`

## Diagram Descriptions

### System Context (Level 1)
Shows the RealEstate system in the context of users and external systems. Includes property owners, property seekers, administrators, and external services like payment systems and maps.

### Container (Level 2)
Shows the high-level architecture with containers like Web Application, API, Database, Cache Layer, and Background Jobs, along with their relationships.

### API Components (Level 3)
Detailed view of the RealEstate API showing controllers, middleware, handlers, domain services, repositories, and their interactions following Clean Architecture principles.

### Web App Components (Level 3)
Shows the React SPA components including pages, services, state management, and client-side integrations.

### Deployment
Illustrates the production deployment architecture with load balancing, auto-scaling, database replication, monitoring, and CI/CD pipeline.

## Customization

You can modify any of the `.puml` files to:
- Change colors using PlantUML color codes
- Add or remove components
- Modify relationships and descriptions
- Adjust layout using PlantUML directives

Refer to the [C4-PlantUML documentation](https://github.com/plantuml-stdlib/C4-PlantUML) for more customization options.