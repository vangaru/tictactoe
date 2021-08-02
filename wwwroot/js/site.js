const X_CLASS = 'x'
const CIRCLE_CLASS = 'circle'
const WINNING_COMBINATIONS = [
    [0, 1, 2],
    [3, 4, 5],
    [6, 7, 8],
    [0, 3, 6],
    [1, 4, 7],
    [2, 5, 8],
    [0, 4, 8],
    [2, 4, 6]
]

const cellElements = document.querySelectorAll('[data-cell]')
const board = document.getElementById('board')
const winningMessageElement = document.getElementById('winningMessage')
const winningMessageTextElement = document.querySelector('[data-winning-message-text]')
let circleTurn
let firstMove

const hubConnection = new signalR.HubConnectionBuilder().withUrl("/ticTacToe").build()

function startGame() {
    circleTurn = false
    cellElements.forEach(cell => {
        cell.classList.remove(X_CLASS)
        cell.classList.remove(CIRCLE_CLASS)
    })
    setBoardHoverClass()
    winningMessageElement.classList.remove('show')
    const firstMoveCellId = board.classList[1]
    const firstMoveCell = document.getElementById(firstMoveCellId)
    placeMark(firstMoveCell, X_CLASS)
    swapTurns()
}

function handleClick(e) {
    const cell = e.target
    const cellId = cell.getAttribute("id")
    const currentClass = circleTurn ? CIRCLE_CLASS : X_CLASS
    hubConnection.invoke("SendMove", currentClass, parseInt(cellId), document.URL)
    placeMark(cell, currentClass)
    if (checkWin(currentClass)) {
        endGame(false)
    } else if (isDraw()) {
        endGame(true)
    } else {
        swapTurns()
    }
    cellElements.forEach(cell => {
        cell.removeEventListener('click', handleClick)
    })
}

function placeMark(cell, currentClass) {
    cell.classList.add(currentClass)
}

function checkWin(currentClass) {
    return WINNING_COMBINATIONS.some(combination => {
        return combination.every(index => {
            return cellElements[index].classList.contains(currentClass)
        })
    })
}

function isDraw() {
    return [...cellElements].every(cell => {
        return cell.classList.contains(X_CLASS) || cell.classList.contains(CIRCLE_CLASS)
    })
}

function endGame(draw) {
    if (draw) {
        winningMessageTextElement.innerText = 'Draw!'
    } else {
        winningMessageTextElement.innerText = `${circleTurn ? "O's" : "X's"} Wins!`
    }
    winningMessageElement.classList.add('show')
    hubConnection.invoke("SendGameResult", draw, document.URL);
}

function swapTurns() {
    circleTurn = !circleTurn
}

function setBoardHoverClass() {
    board.classList.remove(X_CLASS)
    board.classList.remove(CIRCLE_CLASS)
    if (circleTurn) {
        board.classList.add(CIRCLE_CLASS)
    } else {
        board.classList.add(X_CLASS)
    }
}

hubConnection.on("ReceiveConnectedUser", () => {
    hubConnection.invoke("EnableEventListeners", document.URL);
})

hubConnection.on("ReceiveEventListeners", (url) => {
    if (urlIdsAreEqual(document.URL, url)) {
        setBoardHoverClass()
        cellElements.forEach(cell => {
            cell.addEventListener('click', handleClick)
        })
    }
})

hubConnection.on("ReceiveMove", (currentClass, cellId, url) => {
    if (urlIdsAreEqual(document.URL, url)) {
        const cell = document.getElementById(cellId)
        swapTurns()
        setBoardHoverClass()
        placeMark(cell, currentClass)
        cellElements.forEach(cell => {
            cell.addEventListener('click', handleClick)
        })
    }
})

hubConnection.on("ReceiveGameResult", (draw, url) => {
    if (urlIdsAreEqual(document.URL, url)) {
        if (draw) {
            winningMessageTextElement.innerText = 'Draw!'
        } else {
            winningMessageTextElement.innerText = `${!circleTurn ? "O's" : "X's"} Wins!`
        }
        winningMessageElement.classList.add('show')
    }
})

function urlIdsAreEqual(url1, url2) {
    url1 = url1.split('/')
    url2 = url2.split('/')
    return url1[5][0] === url2[5][0]
} 

hubConnection.start()

startGame()