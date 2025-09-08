# API Reference

Welcome to the RealEstate API Reference! Here you can explore and test all available endpoints interactively.

## Interactive API Documentation

Our API documentation is powered by Scalar UI, providing an intuitive interface to explore and test all endpoints directly integrated into the API server.

### 🚀 Access API Documentation

<div style={{
  border: '2px solid #0066cc',
  borderRadius: '12px',
  padding: '30px',
  textAlign: 'center',
  margin: '30px 0',
  background: 'linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%)'
}}>

### **📋 Interactive API Testing**
**Full-featured Scalar UI Interface**

Access the complete API documentation and testing interface:

**[🚀 Open API Documentation →](http://localhost:5000/scalar)**

*Available when running the application with Docker Compose*

</div>

## API Overview

### **Million Realty API**
- **Version:** 1.0.0  
- **Base URL:** `http://localhost:5000`  
- **Documentation:** Interactive Swagger/OpenAPI 3.0

### **Key Features**

- ✅ **Full CRUD Operations** - Properties and Owners management
- ✅ **Advanced Filtering** - Search properties by multiple criteria  
- ✅ **Partial Updates** - PATCH operations for efficient updates
- ✅ **Real-time Validation** - Comprehensive input validation
- ✅ **Audit Trails** - Complete property change tracking
- ✅ **Authentication Ready** - JWT Bearer token support

### **Available Endpoints**

| Resource | Endpoints | Description |
|----------|-----------|-------------|
| **Properties** | `GET, POST, PUT, PATCH, DELETE /api/properties` | Manage property listings |
| **Owners** | `GET, POST, PUT, PATCH, DELETE /api/owners` | Manage property owners |
| **Property Images** | `GET, POST, DELETE /api/properties/{id}/images` | Handle property photos |
| **Property Traces** | `GET /api/properties/{id}/traces` | View property history |
| **Health** | `GET /health` | API health status |

### **Quick Start**

1. **Start the services:**
   ```bash
   docker-compose up -d
   ```

2. **Access the API documentation:**
   - Interactive testing: http://localhost:4000
   - API built-in docs: http://localhost:5000/scalar

3. **Test basic endpoints:**
   ```bash
   # Health check
   curl http://localhost:5000/health
   
   # Get all properties
   curl http://localhost:5000/api/properties
   
   # Get all owners  
   curl http://localhost:5000/api/owners
   ```

### **Authentication**

The API supports JWT Bearer token authentication. To use authenticated endpoints:

1. Include the `Authorization` header: `Bearer <your-token>`
2. Use the interactive documentation to test with authentication
3. Refer to the authentication endpoints in the full API documentation

---

## 📚 Related Documentation

- [Architecture Overview](./architecture/overview.md)
- [Getting Started Guide](./getting-started/installation.md)
- [Architecture Decision Records](./adrs/index.md)

*For detailed endpoint specifications, parameters, and examples, please use the interactive API documentation linked above.*