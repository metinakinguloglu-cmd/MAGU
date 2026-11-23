const HOST = "com.magu.companion";
let port = null;
const pending = new Map();

function connect() {
  if (port) return port;
  port = chrome.runtime.connectNative(HOST);
  port.onMessage.addListener(message => {
    const callback = pending.get(message.id);
    if (!callback) return;
    if (message.type === "progress") {
      callback(message);
      return;
    }
    pending.delete(message.id);
    callback(message);
  });
  port.onDisconnect.addListener(() => { port = null; });
  return port;
}

chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
  if (message?.type !== "magu-native" || !message.payload?.id) return false;
  try {
    const native = connect();
    const id = message.payload.id;
    pending.set(id, value => sendResponse(value));
    native.postMessage(message.payload);
  } catch (error) {
    sendResponse({type: "error", id: message.payload.id, ok: false, error: error.message});
  }
  return true;
});

chrome.action.onClicked.addListener(tab => {
  if (tab?.id) chrome.tabs.sendMessage(tab.id, {type: "toggle-magu"}).catch(() => {});
});
