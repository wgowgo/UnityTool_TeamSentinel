let ws = null;
let clients = new Map();
let cpuChart, memoryChart;
let cpuData = [];
let memoryData = [];
let timeLabels = [];
let reconnectAttempts = 0;
const maxReconnectAttempts = 10;
const reconnectDelay = 3000;

function initCharts() {
    const cpuCtx = document.getElementById('cpu-chart').getContext('2d');
    const memCtx = document.getElementById('memory-chart').getContext('2d');

    cpuChart = new Chart(cpuCtx, {
        type: 'line',
        data: {
            labels: timeLabels,
            datasets: []
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true,
                    max: 100,
                    ticks: { color: '#fff' },
                    grid: { color: '#333' }
                },
                x: {
                    ticks: { color: '#fff' },
                    grid: { color: '#333' }
                }
            },
            plugins: {
                legend: { labels: { color: '#fff' } }
            }
        }
    });

    memoryChart = new Chart(memCtx, {
        type: 'line',
        data: {
            labels: timeLabels,
            datasets: []
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true,
                    max: 100,
                    ticks: { color: '#fff' },
                    grid: { color: '#333' }
                },
                x: {
                    ticks: { color: '#fff' },
                    grid: { color: '#333' }
                }
            },
            plugins: {
                legend: { labels: { color: '#fff' } }
            }
        }
    });
}

function updateStats() {
    let total = clients.size;
    let unityRunning = 0;
    let totalCpu = 0;
    let totalMemory = 0;

    clients.forEach(client => {
        if (client.UnityRunning) unityRunning++;
        totalCpu += client.CpuUsage || 0;
        totalMemory += client.MemoryUsage || 0;
    });

    document.getElementById('total-clients').textContent = total;
    document.getElementById('unity-running').textContent = unityRunning;
    document.getElementById('avg-cpu').textContent = total > 0 ? (totalCpu / total).toFixed(1) + '%' : '0%';
    document.getElementById('avg-memory').textContent = total > 0 ? (totalMemory / total).toFixed(1) + '%' : '0%';
}

function updateCharts(data) {
    const now = new Date().toLocaleTimeString();
    timeLabels.push(now);
    if (timeLabels.length > 20) timeLabels.shift();

    let cpuDatasets = [];
    let memDatasets = [];

    clients.forEach((client, pcName) => {
        cpuDatasets.push({
            label: pcName,
            data: client.cpuHistory || [],
            borderColor: getColor(pcName),
            tension: 0.1
        });
        memDatasets.push({
            label: pcName,
            data: client.memoryHistory || [],
            borderColor: getColor(pcName),
            tension: 0.1
        });
    });

    cpuChart.data.labels = timeLabels;
    cpuChart.data.datasets = cpuDatasets;
    cpuChart.update();

    memoryChart.data.labels = timeLabels;
    memoryChart.data.datasets = memDatasets;
    memoryChart.update();
}

function getColor(str) {
    let hash = 0;
    for (let i = 0; i < str.length; i++) {
        hash = str.charCodeAt(i) + ((hash << 5) - hash);
    }
    const hue = hash % 360;
    return `hsl(${hue}, 70%, 50%)`;
}

function getStatusClass(status) {
    if (status.includes('정상')) return 'status-normal';
    if (status.includes('경고') || status.includes('낮음')) return 'status-warning';
    return 'status-error';
}

function connectWebSocket() {
    try {
        ws = new WebSocket("ws://127.0.0.1:6000");
        setupWebSocketHandlers();
    } catch (error) {
        console.error('WebSocket 연결 실패:', error);
        scheduleReconnect();
    }
}

function setupWebSocketHandlers() {
    ws.onopen = () => {
        console.log('WebSocket connected');
        reconnectAttempts = 0;
        if (!cpuChart || !memoryChart) {
            initCharts();
        }
    };

    ws.onmessage = (e) => {
    let data = JSON.parse(e.data);
    
    if (!clients.has(data.PcName)) {
        clients.set(data.PcName, { cpuHistory: [], memoryHistory: [] });
    }

    let client = clients.get(data.PcName);
    client.cpuHistory = client.cpuHistory || [];
    client.memoryHistory = client.memoryHistory || [];

    client.cpuHistory.push(data.CpuUsage || 0);
    client.memoryHistory.push(data.MemoryUsage || 0);

    if (client.cpuHistory.length > 20) {
        client.cpuHistory.shift();
        client.memoryHistory.shift();
    }

    Object.assign(client, data);
    clients.set(data.PcName, client);

    updateStats();
    updateCharts(data);

    let html = '';
    clients.forEach((client, pcName) => {
        html += `<div class='box'>
            <h3>${pcName}</h3>
            <div class="metric status-${getStatusClass(client.Status)}">상태: ${client.Status}</div>
            <div class="metric">Unity: ${client.UnityRunning ? '실행중' : '미실행'}</div>
            <div class="metric">Idle: ${client.IdleMinutes}분</div>
            <div class="metric">Focus: ${(client.FocusLevel * 100).toFixed(0)}%</div>
            <div class="metric">CPU: ${client.CpuUsage?.toFixed(1) || 0}%</div>
            <div class="metric">메모리: ${client.MemoryUsage?.toFixed(1) || 0}%</div>
            <div class="metric">Unity CPU: ${client.UnityCpuUsage?.toFixed(1) || 0}%</div>
            <div class="metric">Unity 메모리: ${client.UnityMemoryUsage?.toFixed(1) || 0}MB</div>
            <div class="metric">오늘 작업: ${client.TodayWorkMinutes || 0}분</div>
            <div style="margin-top:10px; color:#888; font-size:12px;">
                Active: ${client.ActiveWindow || 'N/A'}<br>
                ${client.Events && client.Events.length > 0 ? 'Events: ' + client.Events.length : ''}
            </div>
        </div>`;
    });
    document.getElementById("clients").innerHTML = html;
};

    ws.onerror = (error) => {
        console.error('WebSocket error:', error);
    };

    ws.onclose = () => {
        console.log('WebSocket disconnected');
        ws = null;
        scheduleReconnect();
    };
}

function scheduleReconnect() {
    if (reconnectAttempts >= maxReconnectAttempts) {
        console.error('최대 재연결 시도 횟수 초과');
        document.getElementById('clients').innerHTML = '<div style="color:red; padding:20px;">서버에 연결할 수 없습니다. 페이지를 새로고침하세요.</div>';
        return;
    }

    reconnectAttempts++;
    const delay = reconnectDelay * reconnectAttempts; // 지수 백오프
    console.log(`${delay/1000}초 후 재연결 시도 (${reconnectAttempts}/${maxReconnectAttempts})...`);
    
    setTimeout(() => {
        if (!ws || ws.readyState === WebSocket.CLOSED) {
            connectWebSocket();
        }
    }, delay);
}

// 초기 연결
connectWebSocket();

