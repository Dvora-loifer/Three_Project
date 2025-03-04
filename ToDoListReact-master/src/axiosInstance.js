import axios from 'axios';

const axiosInstance = axios.create({
    // baseURL: 'http://localhost:5250', // החלף בכתובת ה-URL של ה-API שלך
    baseURL:"https://three-project-iuho.onrender.com"

    // ,

});

// הוספת אינטרצפטור לטיפול בשגיאות 401
axiosInstance.interceptors.response.use(
    response => response,
    error => {
        if (error.response && error.response.status === 401) {
            window.location.href = '/Login'; // הפנה לדף התחברות
        }
        return Promise.reject(error);
    }
);

export default axiosInstance;