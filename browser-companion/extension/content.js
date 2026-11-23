(() => {
  if (window.top !== window) return;
  const host = document.createElement("div");
  host.id = "magu-browser-companion";
  const shadow = host.attachShadow({mode: "open"});
  shadow.innerHTML = `<style>
    :host{all:initial;position:fixed;z-index:2147483647;right:22px;bottom:76px;font-family:Segoe UI,Arial,sans-serif;color:#f7f9ff}
    .button{width:42px;height:42px;border:1px solid #347dff;border-radius:14px;background:linear-gradient(135deg,#08143a,#112a75 55%,#721eff);box-shadow:0 8px 24px #0008;cursor:pointer;display:grid;place-items:center;transition:.18s transform,.18s box-shadow}.button:hover{transform:translateY(-2px);box-shadow:0 12px 30px #182f9dcc}.button svg{width:26px;height:26px}
    .panel{display:none;width:260px;margin-bottom:9px;padding:14px;border:1px solid #2b5bcc;border-radius:18px;background:linear-gradient(160deg,#071332f5,#101d48f5 65%,#24105cf5);box-shadow:0 14px 40px #000b}.panel.open{display:block}.title{font-size:14px;font-weight:700;margin-bottom:4px}.subtitle{font-size:11px;color:#aabceb;line-height:1.35;margin-bottom:12px}.row{display:flex;gap:6px;flex-wrap:wrap;margin:6px 0}.option{flex:1;min-width:72px;border:1px solid #315db8;border-radius:9px;padding:7px 5px;background:#0e2050;color:#f7f9ff;font-size:11px;cursor:pointer}.option:hover,.option.selected{border-color:#7b3cff;background:#1d3b83}.muted{font-size:10px;color:#91a8d9}.error{color:#ffb7c5;font-size:11px}
  </style><div class="panel"><div class="title">MAGU ile indir</div><div class="subtitle">Bir seçenek seçin. İndirme doğrudan MAGU klasörünüze kaydedilir.</div><div class="formats row"><button class="option" data-kind="mp3">MP3 indir</button><button class="option" data-kind="mp4">MP4 indir</button><button class="option" data-kind="wav">WAV indir</button><button class="option" data-kind="thumbnail">Kapak fotoğrafını kaydet</button></div><div class="qualities row"></div><div class="state muted"></div></div><button class="button" title="MAGU"><svg viewBox="0 0 32 32" aria-hidden="true"><defs><linearGradient id="g" x1="0" y1="0" x2="1" y2="1"><stop stop-color="#11d8ee"/><stop offset=".55" stop-color="#2671ff"/><stop offset="1" stop-color="#9a15ff"/></linearGradient></defs><path d="M5 24V13l7-8 7 8 5-5 3 4v12l-8-8-5 5-5-5v8z" fill="none" stroke="url(#g)" stroke-width="3" stroke-linejoin="round"/></svg></button>`;
  document.documentElement.appendChild(host);
  const button=shadow.querySelector(".button"),panel=shadow.querySelector(".panel"),formats=shadow.querySelector(".formats"),qualities=shadow.querySelector(".qualities"),state=shadow.querySelector(".state");
  let video=null,requestId=0;
  const id=()=>`yt-${Date.now()}-${++requestId}`;
  const currentUrl=()=>location.href;
  function request(payload){return new Promise(resolve=>chrome.runtime.sendMessage({type:"magu-native",payload},resolve));}
  async function togglePanel(){panel.classList.toggle("open");if(panel.classList.contains("open"))state.textContent="Bir indirme seçin.";}
  button.onclick=togglePanel;
  chrome.runtime.onMessage.addListener(message=>{if(message?.type==="toggle-magu")togglePanel();});
  formats.querySelectorAll("[data-kind]").forEach(option=>option.onclick=async()=>{formats.querySelectorAll("button").forEach(x=>x.disabled=true);state.textContent=`${option.textContent} başlatılıyor…`;const result=await request({type:"download",id:id(),url:currentUrl(),kind:option.dataset.kind});if(result?.ok)state.textContent="İndirme MAGU'da başladı.";else{state.innerHTML=`<span class="error">${result?.error||"İndirme başlatılamadı."}</span>`;formats.querySelectorAll("button").forEach(x=>x.disabled=false);}});
  const observe=()=>{video=document.querySelector("video");host.style.display=video?"block":"none";};new MutationObserver(observe).observe(document.documentElement,{childList:true,subtree:true});observe();
})();
