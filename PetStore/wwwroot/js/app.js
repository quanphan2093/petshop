document.addEventListener("DOMContentLoaded", () => {
    const userInput = document.getElementById("userInput");
    const sendBtn = document.getElementById("sendBtn");
    const chatDisplay = document.getElementById("chatDisplay");

    let conversationHistory = [
        { role: "system", content: "You are a helpful assistant." }
    ];

    const displayMessage = (sender, message) => {
        const messageDiv = document.createElement("div");
        messageDiv.classList.add("message", sender);
        messageDiv.innerHTML = `<p><strong>${sender}:</strong> ${message.replace(/\n/g, '<br>')}</p>`;
        chatDisplay.appendChild(messageDiv);
        chatDisplay.scrollTop = chatDisplay.scrollHeight;
    };

    const fetchAIResponse = async (userMessage) => {
        displayMessage("FurFriends", "Loading...");

        conversationHistory.push({ role: "user", content: userMessage });

        try {
            const response = await fetch("/Common/Chat", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ UserMessage: userMessage })
            });
            if (!response.ok) console.log(response);
            if (!response.ok) throw new Error("Error fetching response from server");

            const data = await response.json();
            const aiMessage = data.choices[0].message.content.trim();

            chatDisplay.lastChild.remove();
            displayMessage("FurFriends", aiMessage);
            conversationHistory.push({ role: "assistant", content: aiMessage });

        } catch (error) {
            displayMessage("Error", error.message);
        }
    };

    sendBtn.addEventListener("click", () => {
        const userMessage = userInput.value.trim();
        if (userMessage) {
            displayMessage("You", userMessage);
            userInput.value = "";
            fetchAIResponse(userMessage);
        }
    });

    userInput.addEventListener("keypress", (event) => {
        if (event.key === "Enter") sendBtn.click();
    });
});