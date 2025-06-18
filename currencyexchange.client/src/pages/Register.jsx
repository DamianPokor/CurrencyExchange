import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import '../css/global.css';
import styles from '../css/Register.module.css';

const Register = () => {
    const navigate = useNavigate();

    const [registerModel, setRegisterModel] = useState({
        firstName: '',
        lastName: '',
        email: '',
        username: '',
        password: '',
        confirmPassword: '',
        acceptTerms: false,
    });

    const [errorMessage, setErrorMessage] = useState('');
    const [successMessage, setSuccessMessage] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    const handleChange = (e) => {
        const { name, value, type, checked } = e.target;
        setRegisterModel((prev) => ({
            ...prev,
            [name]: type === 'checkbox' ? checked : value,
        }));
    };

    const validate = () => {
        const { firstName, lastName, email, username, password, confirmPassword, acceptTerms } = registerModel;
        if (!firstName || !lastName || !email || !username || !password || !confirmPassword) {
            return 'All fields are required.';
        }
        if (!email.includes('@')) {
            return 'Please enter a valid email address.';
        }
        if (password.length < 6) {
            return 'Password must be at least 6 characters.';
        }
        if (password !== confirmPassword) {
            return 'Passwords do not match.';
        }
        if (!acceptTerms) {
            return 'You must accept the terms to register.';
        }
        return '';
    };

    const handleRegister = async (e) => {
        e.preventDefault();
        setIsLoading(true);
        setErrorMessage('');
        setSuccessMessage('');

        
        console.log('HandleRegisterStarted');
    


        const validationError = validate();
        if (validationError) {
            setErrorMessage(validationError);
            setIsLoading(false);
            return;
        }

        try {
            const response = await fetch(`${process.env.REACT_APP_API_URL}/api/auth/register`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(registerModel),
            });

            if (response.ok) {
                setSuccessMessage('Account created successfully! You can now sign in.');
                setTimeout(() => navigate('/'), 2000);
            } else {
                const data = await response.json();
                setErrorMessage(data.message || 'Registration failed. Try again.');
            }
        } catch (err) {
            setErrorMessage('An error occurred. Please try again later.');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className={styles['register-container']}>
            <div className={styles['header']}>
                <div className={styles['logo-section']}>
                    <div className={styles['logo']}>
                        <div className={styles['logo-icon']}>$</div>
                    </div>
                </div>
                <div className={styles['header-buttons']}>
                    <button className={`${styles['btn']} ${styles['btn-secondary']}`} onClick={() => navigate('/')}>Back to Home</button>
                    <button className={`${styles['btn']} ${styles['btn-secondary']}`} onClick={() => navigate('/')}>Login</button>
                </div>
            </div>

            <div className={styles['register-card']}>
                <form onSubmit={handleRegister} className={styles['register-form']}>
                    <div className={styles['logo-section']}>
                        <div className={`${styles['logo-icon']} ${styles['large']}`}>$</div>
                        <h2>Create Account</h2>
                        <p>Sign up for a new account</p>
                    </div>

                    <div className={styles['form-row']}>
                        <div className={styles['form-group']}>
                            <div className={styles['input-container']}>
                                <span className={styles['input-icon']}>👤</span>
                                <input
                                    name="firstName"
                                    value={registerModel.firstName}
                                    onChange={handleChange}
                                    placeholder="First Name"
                                    className={styles['form-input']}
                                    required
                                />
                            </div>
                        </div>

                        <div className={styles['form-group']}>
                            <div className={styles['input-container']}>
                                <span className={styles['input-icon']}>👤</span>
                                <input
                                    name="lastName"
                                    value={registerModel.lastName}
                                    onChange={handleChange}
                                    placeholder="Last Name"
                                    className={styles['form-input']}
                                    required
                                />
                            </div>
                        </div>
                    </div>

                    <div className={styles['form-group']}>
                        <div className={styles['input-container']}>
                            <span className={styles['input-icon']}>📧</span>
                            <input
                                type="email"
                                name="email"
                                value={registerModel.email}
                                onChange={handleChange}
                                placeholder="Email Address"
                                className={styles['form-input']}
                                required
                            />
                        </div>
                    </div>

                    <div className={styles['form-group']}>
                        <div className={styles['input-container']}>
                            <span className={styles['input-icon']}>👤</span>
                            <input
                                name="username"
                                value={registerModel.username}
                                onChange={handleChange}
                                placeholder="Username"
                                className={styles['form-input']}
                                required
                            />
                        </div>
                    </div>

                    <div className={styles['form-row']}>
                        <div className={styles['form-group']}>
                            <div className={styles['input-container']}>
                                <span className={styles['input-icon']}>🔒</span>
                                <input
                                    type="password"
                                    name="password"
                                    value={registerModel.password}
                                    onChange={handleChange}
                                    placeholder="Password"
                                    className={styles['form-input']}
                                    required
                                />
                            </div>
                        </div>

                        <div className={styles['form-group']}>
                            <div className={styles['input-container']}>
                                <span className={styles['input-icon']}>🔒</span>
                                <input
                                    type="password"
                                    name="confirmPassword"
                                    value={registerModel.confirmPassword}
                                    onChange={handleChange}
                                    placeholder="Confirm Password"
                                    className={styles['form-input']}
                                    required
                                />
                            </div>
                        </div>
                    </div>

                    <div className={`${styles['form-group']} ${styles['checkbox-group']}`}>
                        <label className={styles['checkbox-container']}>
                            <input
                                type="checkbox"
                                name="acceptTerms"
                                checked={registerModel.acceptTerms}
                                onChange={handleChange}
                                className={styles['checkbox-input']}
                            />
                            <span className={styles['checkmark']}></span>
                            <span className={styles['checkbox-text']}>
                                I agree to the <a href="/terms" target="_blank">Terms of Service</a> and <a href="/privacy" target="_blank">Privacy Policy</a>
                            </span>
                        </label>
                    </div>

                    <button type="submit" className={`${styles['btn']} ${styles['btn-primary']} ${styles['btn-register']}`} disabled={isLoading}>
                        {isLoading ? 'Creating Account...' : 'Create Account'}
                    </button>

                    {errorMessage && <div className={styles['error-message']}>{errorMessage}</div>}
                    {successMessage && <div className={styles['success-message']}>{successMessage}</div>}

                    <div className={styles['divider']}><span>or</span></div>

                    <button
                        type="button"
                        className={`${styles['btn']} ${styles['btn-outline']} ${styles['btn-login-redirect']}`}
                        onClick={() => navigate('/login')}
                    >
                        Already have an account? Sign In
                    </button>

                    {/*<button type="button" className={styles['btn-google']} onClick={() => alert('Google sign up clicked')}>
                        <img src="data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMTgiIGhlaWdodD0iMTgi..." alt="Google" />
                        Sign Up with Google
                    </button>*/}
                </form>
            </div>
            <p>{process.env.REACT_APP_API_URL}</p>
        </div>
    );
};

export default Register;
