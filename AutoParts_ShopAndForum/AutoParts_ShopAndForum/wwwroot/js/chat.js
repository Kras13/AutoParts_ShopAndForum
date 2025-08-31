const currentUserId = document.querySelector("#currentUserId").value;
const isCurrentUserSeller = document.querySelector("#currentUserSeller").value;

let currentCompanyUserId = null;
let currentCompanyUsername = null;
let timeoutHandle = null;

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: retryContext => {
            return 2000;
        }
    })
    .build();

$("#acceptBtn").on("click", function() {
    connection.invoke("AcceptPrivateChat", currentCompanyUserId).catch(err => console.error(err.toString()));
});

$("#declineBtn").on("click", function() {
    connection.invoke("DeclinePrivateChat", currentCompanyUserId).catch(err => console.error(err.toString()));
});

function startChat(userId, userName) {
    currentCompanyUserId = userId;
    currentCompanyUsername = userName;

    const chatMessages = $("#chatMessages");

    $(".chat-request-buttons").hide();
    $(".chat-input-area").hide();

    $("#liveChat").text(`Онлайн чат с ${userName}`);
    chatMessages.empty();
    $("#chatPopup").fadeIn();

    appendSystemMessage(`Моля, изчакайте докато ${userName} приеме вашата заявка...`);
    startRequestTimer();
    
    connection.invoke("RequestPrivateChat", userId).catch(err => console.error(err.toString()));
}

connection.on("ReceiveChatRequest", function (initiatorId, initiatorUsername) {
    currentCompanyUserId = initiatorId;
    currentCompanyUsername = initiatorUsername;

    $(".chat-request-buttons").show();
    $(".chat-input-area").hide();

    $("#liveChat").text(`Нова заявка от ${initiatorUsername}`);
    $("#chatMessages").empty();
    $("#chatPopup").fadeIn();

    appendSystemMessage(`Моля, приемете или откажете заявката...`);
    startRequestTimer();
});

connection.on("ChatAccepted", function (otherUserId) {
    clearInterval(timeoutHandle);
    
    const chatMessages = $("#chatMessages");

    $(".chat-request-buttons").hide();
    $(".chat-input-area").show();
    
    if (isCurrentUserSeller) { // accepted from myself -> admin
        $("#liveChat").text(`Онлайн чат с ${currentCompanyUsername}`);
        
        appendSystemMessage("Заявката е приета. Можете да започнете разговор.");
    }
    else {
        $("#liveChat").text(`Онлайн чат с ${currentCompanyUsername}`);
        
        appendSystemMessage("Вашата заявка беше приета. Започвате разговор.");
    }
});

connection.on("ChatDeclined", function (message) {
    clearInterval(timeoutHandle);
    
    $("#chatMessages").empty();

    appendSystemMessage(message);

    setTimeout(() => {
        $("#chatPopup").fadeOut();
        
        currentCompanyUserId = null;
        currentCompanyUsername = null;
    }, 3000);
});

function startRequestTimer() {
    let secondsLeft = 10;

    const counterElement = $("<div class='text-secondary text-center my-2'>Оставащо време: <span id='timeout-counter'>10</span>s</div>");
    $("#chatMessages").append(counterElement);

    const updateCounter = () => {
        secondsLeft--;
        $("#timeout-counter").text(secondsLeft);
        if (secondsLeft <= 0) {
            clearInterval(timeoutHandle);
        }
    };

    timeoutHandle = setInterval(updateCounter, 1000);
}

connection.on("UpdateSellersList", function (availableSellers) {
    const container = $("#availableSellers");

    container.empty();

    availableSellers.forEach(user => {
        if (user.id === currentUserId)
            return;

        const userDiv = $(`<div class="staff-entry" tabindex="0">${user.email}</div>`);

        userDiv.on("dblclick", function () {
            startChat(user.id, user.email);
        });

        container.append(userDiv);
    });
});

function appendMyMessage(text) {
    const msgHtml = `
 <div class="my-message">
     <div class="message-bubble bg-primary-subtle">
         <p>${text}</p>
     </div>
 </div>
`;
    $("#chatMessages").append(msgHtml);
}

function appendOtherMessage(text) {
    const msgHtml = `
 <div class="other-message">
     <div class="message-bubble">
         <p>${text}</p>
     </div>
 </div>
`;
    $("#chatMessages").append(msgHtml);
}

$("#sendBtn").on("click", function () {
    const messageInput = $("#messageInput");

    const message = messageInput.val().trim();

    if (!message || !currentCompanyUserId)
        return;

    const messageBox = $("#chatMessages");

    appendMyMessage(message);
    messageBox.scrollTop(messageBox[0].scrollHeight);

    connection.invoke("SendPrivateMessage", currentCompanyUserId, message)
        .catch(err => console.error(err.toString()));

    messageInput.val('');
});

$("#closeBtn").on("click", function () {
    if (isCurrentUserSeller) {
        connection.invoke("EndPrivateChatBySeller").catch(err => console.error(err.toString()));
    } else {
        connection.invoke("EndPrivateChat", currentCompanyUserId).catch(err => console.error(err.toString()));
    }

    $("#chatPopup").fadeOut();
});

function appendSystemMessage(text) {
    const msgHtml = `
 <div class="text-danger">
     <p>${text}</p>
 </div>
`;
    $("#chatMessages").append(msgHtml);
}

connection.on("ReceiveSystemMessage", function(message) {
    appendSystemMessage(message);
});

connection.on("ReceivePrivateMessage", function (fromUserId, fromUserEmail, message) {
    currentCompanyUserId = fromUserId;

    const messageBox = $("#chatMessages");

    appendOtherMessage(message);
    messageBox.scrollTop(messageBox[0].scrollHeight);
});

connection.start().catch(err => console.error(err.toString()));