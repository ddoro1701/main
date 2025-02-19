const config = {
  backendUrl: process.env.NODE_ENV === 'production' 
    ? "https://wrexhamuni-ocr-webapp-deeaeydrf2fdcfdy.uksouth-01.azurewebsites.net/" 
    : "http://localhost:5281/api"
};

export default config;