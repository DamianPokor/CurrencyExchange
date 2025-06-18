import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import '../css/global.css';
import styles from '../css/Login.module.css';

const LoginPage = () => {
    const navigate = useNavigate();

    const [loginModel, setLoginModel] = useState({ email: '', password: '' });
    const [errors, setErrors] = useState({});
    const [isLoading, setIsLoading] = useState(false);
    const [errorMessage, setErrorMessage] = useState('');

    const validate = () => {
        const newErrors = {};
        if (!loginModel.email.trim()) {
            newErrors.email = 'Email is required';
        }
        if (!loginModel.password) {
            newErrors.password = 'Password is required';
        } else if (loginModel.password.length < 6) {
            newErrors.password = 'Password must be at least 6 characters';
        }
        return newErrors;
    };

    const handleChange = (e) => {
        setLoginModel({ ...loginModel, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        const validationErrors = validate();
        if (Object.keys(validationErrors).length > 0) {
            setErrors(validationErrors);
            return;
        }

        setErrors({});
        setIsLoading(true);
        setErrorMessage('');

        try {
            const response = await fetch(`${process.env.REACT_APP_API_URL}/api/auth/login`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(loginModel),
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText || 'Login failed');
            }

            const data = await response.json();
            localStorage.setItem('token', data.token);
            navigate('/dashboard');
        } catch (err) {
            setErrorMessage(err.message || 'An error occurred during login. Please try again.');
        } finally {
            setIsLoading(false);
        }
    };

    const handleForgotPassword = (e) => {
        e.preventDefault();
        navigate('/forgot-password');
    };

    const handleNavigateHome = () => navigate('/');
    const handleNavigateRegister = () => navigate('/register');

    return (
        <div className={styles['login-container']}>
            <div className={styles.header}>
                <div className={styles['logo-section']}>
                    <div className={styles.logo}>
                        <div className={styles['logo-icon']}>$</div>
                    </div>
                </div>
                <div className={styles['header-buttons']}>
                    <button className={`${styles.btn} ${styles['btn-secondary']}`} onClick={handleNavigateHome}>Back to Home</button>
                    <button className={`${styles.btn} ${styles['btn-secondary']}`} onClick={handleNavigateRegister}>Register</button>
                </div>
            </div>

            <div className={styles['login-card']}>
                <form onSubmit={handleSubmit} className={styles['login-form']}>
                    <div className={styles['logo-section']}>
                        <div className={`${styles['logo-icon']} ${styles.large}`}>$</div>
                        <h2>Login</h2>
                        <p>Sign in to your account</p>
                    </div>

                    <div className={styles['form-group']}>
                        <div className={styles['input-container']}>
                            <span className={styles['input-icon']}>📧</span>
                            <input
                                type="email"
                                name="email"
                                placeholder="Email"
                                className={styles['form-input']}
                                value={loginModel.email}
                                onChange={handleChange}
                            />
                        </div>
                        {errors.email && <div className={styles['validation-message']}>{errors.email}</div>}
                    </div>

                    <div className={styles['form-group']}>
                        <div className={styles['input-container']}>
                            <span className={styles['input-icon']}>🔒</span>
                            <input
                                type="password"
                                name="password"
                                placeholder="Password"
                                className={styles['form-input']}
                                value={loginModel.password}
                                onChange={handleChange}
                            />
                        </div>
                        {errors.password && <div className={styles['validation-message']}>{errors.password}</div>}
                    </div>

                    <button type="submit" className={`${styles.btn} ${styles['btn-primary']} ${styles['btn-login']}`} disabled={isLoading}>
                        {isLoading ? 'Logging in...' : 'Login'}
                    </button>

                    {errorMessage && <div className={styles['error-message']}>{errorMessage}</div>}

                    <div className={styles['forgot-password']}>
                        <a href="#" onClick={handleForgotPassword}>
                            I forgot my password. Click here to reset
                        </a>
                    </div>

                    <div className={styles.divider}>
                        <span>or</span>
                    </div>

                    <button type="button" className={`${styles.btn} ${styles['btn-outline']} ${styles['btn-register']}`} onClick={handleNavigateRegister}>
                        Register new account
                    </button>
                </form>
            </div>
        </div>
    );
};

export default LoginPage;
