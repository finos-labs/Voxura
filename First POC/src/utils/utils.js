
export const getFunctionDescriptions = () => {
    return [
        {
            name: 'setProductNameFilter',
            description: 'set the product name filter',
            parameters: {
                type: 'object',
                properties: {
                    productName: { type: 'string' },
                },
                required: ['productName'],
            },
        },
        {
            name: 'setBackgroundColor',
            description: 'sets the background color of the page',
            parameters: {
                type: 'object',
                properties: {
                    red: { type: 'number', minimum: 0, maximum: 255 },
                    green: { type: 'number', minimum: 0, maximum: 255 },
                    blue: { type: 'number', minimum: 0, maximum: 255 },
                },
                required: ['red', 'green', 'blue'],
            },
        },
        {
            name: 'setDayOrNightMode',
            description: 'sets day or night mode',
            parameters: {
                type: 'object',
                properties: {
                    mode: { type: 'string', enum: ['day', 'night'] },
                },
                required: ['mode'],
            },
        },
    ];
};

export const prepareChatMessagesForSending = (chatMessages) => {
    return chatMessages.map((message) => {
        if (message.role === 'system') {
            return {
                role: message.role,
                content: message.content,
            };
        } else if (message.role === 'user') {
            return {
                role: message.role,
                content: message.content,
            };
        } else if (message.role === 'assistant') {
            return {
                role: message.role,
                content: message.content,
                function_call: message.function_call,
            };
        } else if (message.role === 'function') {
            return {
                role: message.role,
                content: message.content,
                name: message.name,
            };
        }
    });
};