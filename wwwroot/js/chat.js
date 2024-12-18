document.addEventListener('DOMContentLoaded', function() {
    console.log('Chat script loaded');
    
    const userInput = document.getElementById('userInput');
    const chatMessages = document.getElementById('chatMessages');
    const translateToEnglish = document.getElementById('translateToEnglish');
    const sendButton = document.getElementById('sendButton');

    console.log('Elements found:', {
        userInput: userInput ? 'yes' : 'no',
        chatMessages: chatMessages ? 'yes' : 'no',
        translateToEnglish: translateToEnglish ? 'yes' : 'no',
        sendButton: sendButton ? 'yes' : 'no'
    });

    // Create typing indicator
    const typingIndicator = document.createElement('div');
    typingIndicator.className = 'typing-indicator';
    typingIndicator.innerHTML = '<span></span><span></span><span></span>';

    function addMessage(message, isUser = false) {
        console.log('Adding message:', { message, isUser });
        const messageDiv = document.createElement('div');
        messageDiv.className = `message ${isUser ? 'user-message' : 'bot-message'}`;
        messageDiv.textContent = message;
        chatMessages.appendChild(messageDiv);
        chatMessages.scrollTop = chatMessages.scrollHeight;
    }

    function showTypingIndicator() {
        chatMessages.appendChild(typingIndicator);
        typingIndicator.style.display = 'block';
        chatMessages.scrollTop = chatMessages.scrollHeight;
    }

    function hideTypingIndicator() {
        typingIndicator.style.display = 'none';
        if (typingIndicator.parentNode === chatMessages) {
            chatMessages.removeChild(typingIndicator);
        }
    }

    async function sendMessage(message, shouldTranslate) {
        console.log('Attempting to send message:', { message, shouldTranslate });
        try {
            showTypingIndicator();
            
            const requestBody = {
                message: message,
                translateToEnglish: shouldTranslate
            };
            console.log('Request body:', requestBody);

            const response = await fetch('/api/chat', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                },
                body: JSON.stringify(requestBody)
            });

            console.log('Response status:', response.status);
            const responseText = await response.text();
            console.log('Raw response:', responseText);

            if (!response.ok) {
                throw new Error(`Network response was not ok: ${response.status} ${responseText}`);
            }

            const data = JSON.parse(responseText);
            console.log('Parsed response data:', data);
            
            hideTypingIndicator();
            if (data.success) {
                addMessage(data.message || data.response);
            } else {
                throw new Error(data.message || 'Unknown error');
            }

        } catch (error) {
            console.error('Error in sendMessage:', error);
            hideTypingIndicator();
            addMessage('מצטערים, אירעה שגיאה בעת עיבוד הבקשה שלך. אנא נסה שוב.');
        }
    }

    function handleSendMessage() {
        console.log('handleSendMessage called');
        const message = userInput.value.trim();
        if (message) {
            console.log('Processing message:', message);
            addMessage(message, true);
            sendMessage(message, translateToEnglish.checked);
            userInput.value = '';
        }
    }

    // Handle send button click
    if (sendButton) {
        console.log('Adding click event listener to send button');
        sendButton.onclick = function(e) {
            console.log('Send button clicked');
            e.preventDefault();
            handleSendMessage();
        };
    } else {
        console.error('Send button not found in the DOM');
    }

    // Handle textarea enter key
    if (userInput) {
        console.log('Adding keypress event listener to userInput');
        userInput.onkeypress = function(e) {
            if (e.key === 'Enter' && !e.shiftKey) {
                console.log('Enter key pressed');
                e.preventDefault();
                handleSendMessage();
            }
        };
    } else {
        console.error('UserInput element not found in the DOM');
    }

    // Add initial welcome message
    addMessage('שלום! איך אוכל לעזור לך היום?');
    console.log('Chat initialization complete');
});
