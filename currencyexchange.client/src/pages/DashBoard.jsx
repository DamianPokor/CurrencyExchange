import React, { useState } from 'react';
import '../css/global.css';
import styles from  '../css/DashBoard.module.css';

export default function DashboardPage() {
    const [activeTab, setActiveTab] = useState('MyAccounts');
    const [sortBy, setSortBy] = useState('currency');
    const [showCreateModal, setShowCreateModal] = useState(false);
    const [newAccountCurrency, setNewAccountCurrency] = useState('');
    const [newAccountBalance, setNewAccountBalance] = useState(0);
    const [accounts, setAccounts] = useState([
        { currency: "PLN", balance: 159.99, accountNumber: "03 7483 9405 9382 7028 3944 7876" },
        { currency: "USD", balance: 70.88, accountNumber: "03 6666 0978 6666 2456 3944 7450" },
        { currency: "EUR", balance: 13.20, accountNumber: "42 2222 9405 9382 3337 3944 2789" },
        { currency: "CNY", balance: 999.12, accountNumber: "33 7483 6789 9382 6543 5555 0000" },
        { currency: "NOK", balance: 0.00, accountNumber: "11 3456 3477 777 7654 3944 5467" },
    ]);

    const sortedAccounts = [...accounts].sort((a, b) => {
        if (sortBy === 'balance') return b.balance - a.balance;
        if (sortBy === 'account') return a.accountNumber.localeCompare(b.accountNumber);
        return a.currency.localeCompare(b.currency);
    });

    const createNewAccount = () => {
        if (!newAccountCurrency) return;
        const generateAccountNumber = () => {
            const parts = [rand(10, 99)];
            for (let i = 0; i < 5; i++) parts.push(rand(1000, 9999));
            return parts.join(' ');
        };
        const rand = (min, max) => Math.floor(Math.random() * (max - min + 1)) + min;

        setAccounts(prev => [...prev, {
            currency: newAccountCurrency,
            balance: newAccountBalance,
            accountNumber: generateAccountNumber()
        }]);
        setShowCreateModal(false);
        setNewAccountCurrency('');
        setNewAccountBalance(0);
    };

    const handleLogout = () => {
        window.location.href = '/';
    };

    return (
        <div className={styles['dashboard-container']}>
            <div className={styles.header}>
                <div className={styles['logo-section']}>
                    <div className={styles.logo}>
                        <div className={styles['logo-icon']}>$</div>
                    </div>
                </div>
                <div className={styles['header-buttons']}>
                    {["Exchange", "MyAccounts", "History", "Settings"].map(tab => (
                        <button
                            key={tab}
                            className={`${styles.btn} ${styles['btn-secondary']} ${activeTab === tab ? styles.active : ''}`}
                            onClick={() => setActiveTab(tab)}
                        >
                            {tab}
                        </button>
                    ))}
                    <button className={`${styles.btn} ${styles['btn-secondary']} ${styles.logout}`} onClick={handleLogout}>
                        Logout
                    </button>
                </div>
            </div>


            <div className={styles['main-content']}>
                {activeTab === "MyAccounts" && (
                    <div className={styles['accounts-section']}>
                        <div className={styles['accounts-header']}>
                            <div className={styles['sort-container']}>
                                <label htmlFor="sort-select">Sortuj</label>
                                <select
                                    id="sort-select"
                                    onChange={e => setSortBy(e.target.value)}
                                    className={styles['sort-select']}
                                >
                                    <option value="currency">Currency</option>
                                    <option value="balance">Balance</option>
                                    <option value="account">Account Number</option>
                                </select>
                            </div>
                            <button className={styles['btn'] + ' ' + styles['btn-primary']} onClick={() => setShowCreateModal(true)}>
                                Create new Bank account
                            </button>
                        </div>

                        <div className={styles['accounts-list']}>
                            {sortedAccounts.map((acc, i) => (
                                <div className={styles['account-row']} key={i}>
                                    <div className={styles['account-info']}>
                                        <div className={styles['currency-code']}>{acc.currency}</div>
                                        <div className={styles['balance']}>{acc.balance.toFixed(2)}</div>
                                    </div>
                                    <div className={styles['account-number']}>{acc.accountNumber}</div>
                                    <div className={styles['account-actions']}>
                                        <button className={styles['btn-icon']} title="Edit Account">⚙️</button>
                                    </div>
                                </div>
                            ))}
                        </div>

                        <div className={styles['account-actions-footer']}>
                            <button className={styles['btn'] + ' ' + styles['btn-secondary']}>Add Funds</button>
                            <button className={styles['btn'] + ' ' + styles['btn-secondary']}>Cash out</button>
                        </div>
                    </div>
                )}

                {activeTab === "Exchange" && (
                    <div className={styles['exchange-section']}>
                        <h2>Currency Exchange</h2>
                        <p>Exchange functionality will be implemented here.</p>
                    </div>
                )}

                {activeTab === "History" && (
                    <div className={styles['history-section']}>
                        <h2>Transaction History</h2>
                        <p>Transaction history will be displayed here.</p>
                    </div>
                )}

                {activeTab === "Settings" && (
                    <div className={styles['settings-section']}>
                        <h2>Account Settings</h2>
                        <p>Account settings will be available here.</p>
                    </div>
                )}
            </div>

            {showCreateModal && (
                <div className={styles['modal-overlay']} onClick={() => setShowCreateModal(false)}>
                    <div className={styles['modal-content']} onClick={e => e.stopPropagation()}>
                        <div className={styles['modal-header']}>
                            <h3>Create New Bank Account</h3>
                            <button className={styles['modal-close']} onClick={() => setShowCreateModal(false)}>&times;</button>
                        </div>
                        <div className={styles['modal-body']}>
                            <div className={styles['form-group']}>
                                <label>Currency</label>
                                <select
                                    className={styles['form-select']}
                                    value={newAccountCurrency}
                                    onChange={e => setNewAccountCurrency(e.target.value)}
                                >
                                    <option value="">Select Currency</option>
                                    <option value="USD">USD - US Dollar</option>
                                    <option value="EUR">EUR - Euro</option>
                                    <option value="PLN">PLN - Polish Zloty</option>
                                    <option value="GBP">GBP - British Pound</option>
                                    <option value="CNY">CNY - Chinese Yuan</option>
                                    <option value="NOK">NOK - Norwegian Krone</option>
                                </select>
                            </div>
                            <div className={styles['form-group']}>
                                <label>Initial Balance (Optional)</label>
                                <input
                                    type="number"
                                    className={styles['form-input']}
                                    value={newAccountBalance}
                                    onChange={e => setNewAccountBalance(parseFloat(e.target.value || 0))}
                                    placeholder="0.00"
                                    step="0.01"
                                    min="0"
                                />
                            </div>
                        </div>
                        <div className={styles['modal-footer']}>
                            <button className={styles['btn'] + ' ' + styles['btn-secondary']} onClick={() => setShowCreateModal(false)}>Cancel</button>
                            <button
                                className={styles['btn'] + ' ' + styles['btn-primary']}
                                onClick={createNewAccount}
                                disabled={!newAccountCurrency}
                            >
                                Create Account
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}
