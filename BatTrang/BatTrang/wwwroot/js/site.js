document.addEventListener('DOMContentLoaded', () => {
    const $form = document.getElementById('chatForm');
    const $input = document.getElementById('chatInput');
    const $box = document.getElementById('chatMessages');
    const $typing = document.getElementById('typing');
    if (!$form || !$box) return;

    const messages = [
        { role: 'system', content: 'Bạn là trợ lý Bát Tràng. Trả lời ngắn gọn, thân thiện, ưu tiên tiếng Việt.' }
    ];

    function addMsg(role, text) {
        const wrap = document.createElement('div');
        wrap.className = `msg ${role}`;
        wrap.innerHTML = `<span class="bubble">${text.replace(/[&<>"']/g, m => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[m]))}</span>`;
        $box.appendChild(wrap);
        $box.scrollTop = $box.scrollHeight;
    }

    // Lời chào khi panel mở
    document.getElementById('supportChat')?.addEventListener('shown.bs.offcanvas', () => {
        if ($box.childElementCount === 0) addMsg('ai', 'Xin chào! Mình có thể giúp gì hôm nay?');
        $input?.focus();
    });

    $form.addEventListener('submit', async (e) => {
        e.preventDefault();
        const text = ($input.value || '').trim();
        if (!text) return;

        addMsg('user', text);
        messages.push({ role: 'user', content: text });
        $input.value = '';
        $typing.style.display = 'block';

        try {
            const res = await fetch('/api/ai-chat', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    messages,
                    instructions: 'Bạn là trợ lý của cửa hàng gốm Bát Tràng.'
                })
            });

            const data = await res.json();
            const reply = data?.reply || data?.error || 'Không nhận được phản hồi hợp lệ.';
            addMsg('ai', reply);
            if (!data?.error) messages.push({ role: 'assistant', content: reply });
        } catch (err) {
            addMsg('ai', 'Có lỗi kết nối máy chủ.');
            console.error(err);
        } finally {
            $typing.style.display = 'none';
        }
    });
});
