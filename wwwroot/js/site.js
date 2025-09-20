// Weather App JavaScript functionality

// Auto-focus search input
document.addEventListener('DOMContentLoaded', function() {
    const searchInput = document.querySelector('input[name="city"]');
    if (searchInput) {
        searchInput.focus();
    }
});

// Add loading state to search button
function addLoadingState(button) {
    const originalText = button.innerHTML;
    button.innerHTML = '<span class="loading me-2"></span>Loading...';
    button.disabled = true;
    
    // Remove loading state after 10 seconds as fallback
    setTimeout(() => {
        button.innerHTML = originalText;
        button.disabled = false;
    }, 10000);
}

// Remove loading state from search button
function removeLoadingState(button, originalText) {
    button.innerHTML = originalText;
    button.disabled = false;
}

// Enhanced form submission with loading state
document.addEventListener('DOMContentLoaded', function() {
    const form = document.querySelector('form[asp-action="Search"]');
    if (form) {
        form.addEventListener('submit', function(e) {
            const submitButton = form.querySelector('button[type="submit"]');
            if (submitButton) {
                addLoadingState(submitButton);
            }
        });
    }
});

// Weather chart functionality
function createWeatherChart(data) {
    const ctx = document.getElementById('weatherChart');
    if (!ctx) return;

    // Destroy existing chart if it exists
    if (window.weatherChartInstance) {
        window.weatherChartInstance.destroy();
    }

    window.weatherChartInstance = new Chart(ctx, {
        type: 'line',
        data: {
            labels: data.labels,
            datasets: [{
                label: 'Temperature (°C)',
                data: data.temperatures,
                borderColor: 'rgb(255, 99, 132)',
                backgroundColor: 'rgba(255, 99, 132, 0.2)',
                tension: 0.1,
                yAxisID: 'y',
                fill: false
            }, {
                label: 'Humidity (%)',
                data: data.humidity,
                borderColor: 'rgb(54, 162, 235)',
                backgroundColor: 'rgba(54, 162, 235, 0.2)',
                tension: 0.1,
                yAxisID: 'y1',
                fill: false
            }, {
                label: 'Wind Speed (m/s)',
                data: data.windSpeed,
                borderColor: 'rgb(255, 205, 86)',
                backgroundColor: 'rgba(255, 205, 86, 0.2)',
                tension: 0.1,
                yAxisID: 'y2',
                fill: false
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            interaction: {
                mode: 'index',
                intersect: false,
            },
            plugins: {
                legend: {
                    position: 'top',
                },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    titleColor: 'white',
                    bodyColor: 'white',
                    borderColor: 'rgba(255, 255, 255, 0.2)',
                    borderWidth: 1
                }
            },
            scales: {
                x: {
                    display: true,
                    title: {
                        display: true,
                        text: 'Time',
                        color: '#666'
                    },
                    grid: {
                        color: 'rgba(0, 0, 0, 0.1)'
                    }
                },
                y: {
                    type: 'linear',
                    display: true,
                    position: 'left',
                    title: {
                        display: true,
                        text: 'Temperature (°C)',
                        color: '#666'
                    },
                    grid: {
                        color: 'rgba(0, 0, 0, 0.1)'
                    }
                },
                y1: {
                    type: 'linear',
                    display: true,
                    position: 'right',
                    title: {
                        display: true,
                        text: 'Humidity (%)',
                        color: '#666'
                    },
                    grid: {
                        drawOnChartArea: false,
                    },
                },
                y2: {
                    type: 'linear',
                    display: false,
                    position: 'right',
                    title: {
                        display: true,
                        text: 'Wind Speed (m/s)',
                        color: '#666'
                    },
                    grid: {
                        drawOnChartArea: false,
                    },
                }
            }
        }
    });
}

// Load weather chart data
function loadWeatherChart(city) {
    if (!city) return;

    const chartContainer = document.getElementById('weatherChart');
    if (!chartContainer) return;

    // Show loading state
    chartContainer.innerHTML = '<div class="text-center p-4"><div class="loading"></div><p class="mt-2">Loading chart data...</p></div>';

    fetch(`/Weather/GetChartData?city=${encodeURIComponent(city)}`)
        .then(response => response.json())
        .then(data => {
            if (data.error) {
                chartContainer.innerHTML = `<div class="text-center p-4 text-danger"><i class="fas fa-exclamation-triangle"></i><p class="mt-2">${data.error}</p></div>`;
                return;
            }
            chartContainer.innerHTML = '<canvas id="weatherChart" width="400" height="200"></canvas>';
            createWeatherChart(data);
        })
        .catch(error => {
            console.error('Error loading chart data:', error);
            chartContainer.innerHTML = '<div class="text-center p-4 text-danger"><i class="fas fa-exclamation-triangle"></i><p class="mt-2">Failed to load chart data</p></div>';
        });
}

// Auto-refresh weather data every 10 minutes
function startAutoRefresh(city) {
    if (!city) return;

    setInterval(() => {
        // Only refresh if user is still on the page and has searched for a city
        if (document.querySelector('input[name="city"]').value === city) {
            location.reload();
        }
    }, 600000); // 10 minutes
}

// Add smooth scrolling for better UX
function smoothScrollTo(element) {
    element.scrollIntoView({
        behavior: 'smooth',
        block: 'start'
    });
}

// Handle search history clicks
document.addEventListener('DOMContentLoaded', function() {
    const historyLinks = document.querySelectorAll('a[href*="city="]');
    historyLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            const submitButton = document.querySelector('button[type="submit"]');
            if (submitButton) {
                addLoadingState(submitButton);
            }
        });
    });
});

// Add keyboard shortcuts
document.addEventListener('keydown', function(e) {
    // Ctrl/Cmd + K to focus search
    if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
        e.preventDefault();
        const searchInput = document.querySelector('input[name="city"]');
        if (searchInput) {
            searchInput.focus();
            searchInput.select();
        }
    }
    
    // Enter key in search input
    if (e.key === 'Enter' && e.target.name === 'city') {
        const form = e.target.closest('form');
        if (form) {
            const submitButton = form.querySelector('button[type="submit"]');
            if (submitButton) {
                addLoadingState(submitButton);
            }
        }
    }
});

// Add tooltips for weather stats
document.addEventListener('DOMContentLoaded', function() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
});

// Weather condition animations
function addWeatherAnimations() {
    const weatherIcon = document.querySelector('.weather-icon');
    if (weatherIcon) {
        weatherIcon.style.animation = 'pulse 2s infinite';
    }
}

// Add pulse animation CSS
const style = document.createElement('style');
style.textContent = `
    @keyframes pulse {
        0% { transform: scale(1); }
        50% { transform: scale(1.05); }
        100% { transform: scale(1); }
    }
    
    .weather-icon {
        transition: transform 0.3s ease;
    }
    
    .weather-icon:hover {
        transform: scale(1.1);
    }
`;
document.head.appendChild(style);

// Initialize animations when page loads
document.addEventListener('DOMContentLoaded', addWeatherAnimations);
