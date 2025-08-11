const currentUserId = document.querySelector("#currentUserId").value;
const isCurrentUserSeller = document.querySelector("#currentUserSeller").value;

let currentCompanyUserId = null;
let timeoutHandle = null;

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: retryContext => {
            return 2000;
        }
    })
    .build();

function startChat(userId, userName) {
    currentCompanyUserId = userId;
    const chatMessages = $("#chatMessages");

    $("#liveChat").text(`Live chat with ${userName}`);
    chatMessages.empty();
    $("#chatPopup").fadeIn();

    appendSystemMessage(`Моля, изчакайте докато ${userName} приеме вашата заявка...`);

    let secondsLeft = 10;

    const counterElement = $("<div class='text-secondary text-center my-2'>Оставащо време: <span id='timeout-counter'>10</span>s</div>");
    chatMessages.append(counterElement);

    const updateCounter = () => {
        secondsLeft--;
        $("#timeout-counter").text(secondsLeft);
        if (secondsLeft <= 0) {
            clearInterval(timeoutHandle);
        }
    };

    timeoutHandle = setInterval(updateCounter, 1000);

    connection.invoke("RequestPrivateChat", userId).catch(err => console.error(err.toString()));
}

connection.on("UpdateSellersList", function (availableSellers) {
    const container = $("#availableSellers");

    console.log("Update users list called");
    console.log(availableSellers);

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

connection.on("ReceiveChatRequest", function (initiatorId, initiatorEmail) {
    currentCompanyUserId = initiatorId;
    
    if (confirm(`Нова заявка за чат от ${initiatorEmail}. Искаш ли да приемеш?`)) {
        connection.invoke("AcceptPrivateChat", initiatorId).catch(err => console.error(err.toString()));
    } else {
        connection.invoke("DeclinePrivateChat", initiatorId).catch(err => console.error(err.toString()));
    }
});

connection.on("ChatAccepted", function (sellerEmail) {
    clearInterval(timeoutHandle);

    $("#chatMessages").empty();
    $("#chatPopup").fadeIn();    
    $("#liveChat").text(`Live chat with ${sellerEmail}`);
});

connection.on("ChatDeclined", function (message) {
    clearInterval(timeoutHandle);
    appendSystemMessage(message);

    setTimeout(() => {
        $("#chatPopup").fadeOut();
        currentChatUserId = null;
    }, 3000);
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
    console.log("on click fired");

    const messageInput = $("#messageInput");

    const message = messageInput.val().trim();

    console.log("message: " + message);
    console.log("chatting with" + currentCompanyUserId)

    if (!message || !currentCompanyUserId) return;

    console.log("test passed");

    const messageBox = $("#chatMessages");

    appendMyMessage(message);
    messageBox.scrollTop(messageBox[0].scrollHeight);

    connection.invoke("SendPrivateMessage", currentCompanyUserId, message)
        .catch(err => console.error(err.toString()));

    messageInput.val('');
});

$("#closeBtn").on("click", function () {
    console.log("Livechat close called");

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
    console.log("Message received: " + message + " | From " + fromUserId);

    currentCompanyUserId = fromUserId;

    const messageBox = $("#chatMessages");

    appendOtherMessage(message);
    messageBox.scrollTop(messageBox[0].scrollHeight);
});

console.log("Connection start called");

connection.start().catch(err => console.error(err.toString()));