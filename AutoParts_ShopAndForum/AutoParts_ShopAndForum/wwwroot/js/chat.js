const currentUserId = document.querySelector("#currentUserId").value;
let currentChatUserId = null;

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

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

function startChat(userId, userName) {
    currentChatUserId = userId;

    $("#liveChat").text(`Live chat with ${userName}`);
    $("#chatMessages").empty();

    $("#chatPopup").fadeIn();

    connection.invoke("StartPrivateChat", currentUserId, userId).catch(err => console.error(err.toString()));
}

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

    if (!message || !currentChatUserId) return;

    console.log("test passed");

    const messageBox = $("#chatMessages");

    appendMyMessage(message);
    messageBox.scrollTop(messageBox[0].scrollHeight);

    connection.invoke("SendPrivateMessage", currentChatUserId, message)
        .catch(err => console.error(err.toString()));

    messageInput.val('');
});

$("#closeBtn").on("click", function () {
    console.log("Livechat close called");

    connection.invoke("EndPrivateChat", currentChatUserId)
        .catch(err => console.error(err.toString()));

    $("#chatPopup").fadeOut();
});

connection.on("ReceivePrivateMessage", function (fromUserId, fromUserEmail, message) {
    console.log("Message received: " + message + " | From " + fromUserId);

    currentChatUserId = fromUserId;

    $("#chatPopup").fadeIn();
    $("#liveChat").text(`Live chat with ${fromUserEmail}`);

    const messageBox = $("#chatMessages");

    appendOtherMessage(message);
    messageBox.scrollTop(messageBox[0].scrollHeight);
});

console.log("Connection start called");

connection.start().catch(err => console.error(err.toString()));