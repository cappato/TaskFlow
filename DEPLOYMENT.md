# 🚀 TaskFlow Deployment Guide

## Railway Deployment (Recommended)

### Prerequisites
- GitHub account
- Railway account (free): https://railway.app

### Automatic Deployment Setup

1. **Connect GitHub to Railway**
   ```bash
   1. Go to https://railway.app
   2. Sign up with GitHub
   3. Click "New Project"
   4. Select "Deploy from GitHub repo"
   5. Choose your TaskFlow repository
   ```

2. **Configure Environment Variables**
   ```bash
   ASPNETCORE_ENVIRONMENT=Production
   PORT=8080
   ```

3. **Enable Auto-Deploy**
   - Railway automatically detects the Dockerfile
   - Every push to `main` branch triggers deployment
   - Build logs available in Railway dashboard

### Manual Deployment

1. **Install Railway CLI**
   ```bash
   npm install -g @railway/cli
   ```

2. **Login and Deploy**
   ```bash
   railway login
   railway link [your-project-id]
   railway up
   ```

### GitHub Actions CI/CD

The repository includes automatic CI/CD pipeline:

- **On Pull Request**: Runs tests
- **On Push to Main**: Tests → Build → Deploy to Railway

#### Required Secrets

Add these to GitHub repository secrets:

```bash
RAILWAY_TOKEN=your_railway_token
RAILWAY_SERVICE=your_service_id
```

To get Railway token:
1. Go to Railway dashboard
2. Account Settings → Tokens
3. Create new token

## Alternative Platforms

### Azure App Service

1. **Create App Service**
   ```bash
   az webapp create --resource-group myResourceGroup --plan myAppServicePlan --name myapp --runtime "DOTNET|8.0"
   ```

2. **Deploy from Visual Studio**
   - Right-click project → Publish
   - Choose Azure → App Service
   - Configure and deploy

### Render

1. **Connect GitHub**
   - Go to https://render.com
   - New → Web Service
   - Connect GitHub repository

2. **Configure Build**
   ```bash
   Build Command: dotnet publish -c Release -o publish
   Start Command: dotnet publish/TaskFlow.Server.dll
   ```

## Environment Configuration

### Production Settings

The application automatically configures for production:

- **Port**: Uses Railway's dynamic PORT environment variable
- **Database**: SQLite file stored in `/app/data/`
- **CORS**: Configured for Railway domains
- **Logging**: Optimized for production

### Health Check

Available at `/health` endpoint:
```json
{
  "status": "healthy",
  "timestamp": "2024-01-01T00:00:00Z"
}
```

## Monitoring

### Railway Dashboard
- View logs in real-time
- Monitor resource usage
- Configure custom domains
- Set up metrics and alerts

### Application Logs
```bash
railway logs
```

## Troubleshooting

### Common Issues

**🔴 Build Failures**
- Check Dockerfile syntax
- Verify all dependencies are restored
- Review build logs in Railway dashboard

**🔴 Runtime Errors**
- Check environment variables
- Verify database path permissions
- Review application logs

**🔴 CORS Issues**
- Update allowed origins in Program.cs
- Verify Railway domain configuration

### Debug Commands

```bash
# Local build test
docker build -t taskflow .
docker run -p 8080:8080 taskflow

# Railway logs
railway logs --tail

# Railway shell access
railway shell
```

## Performance Optimization

### Railway Free Tier Limits
- 512 MB RAM
- 1 GB disk space
- 500 hours/month

### Optimization Tips
- Enable response compression
- Use SQLite for small datasets
- Implement caching for static data
- Optimize Docker image size

## Security

### Production Checklist
- [ ] HTTPS enabled (automatic on Railway)
- [ ] Environment variables secured
- [ ] Database access restricted
- [ ] CORS properly configured
- [ ] Logging configured (no sensitive data)

## Scaling

### Horizontal Scaling
Railway supports automatic scaling based on traffic.

### Database Considerations
For larger applications, consider:
- PostgreSQL (Railway provides free tier)
- External database services
- Database connection pooling

---

## Quick Deploy Checklist

- [ ] Tests passing locally
- [ ] Environment variables configured
- [ ] Railway project created
- [ ] GitHub repository connected
- [ ] Auto-deploy enabled
- [ ] Health check responding
- [ ] Application accessible via Railway URL

**🎉 Your TaskFlow application is now live!**
