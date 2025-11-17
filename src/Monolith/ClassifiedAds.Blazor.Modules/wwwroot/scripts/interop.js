var interop = interop || {};

interop.focusElementById = function (id) {
    setTimeout(function () {
        var element = document.getElementById(id);
        if (element) {
            element.focus();
        }
    }, 200)
};

interop.downloadFile = function (fileUrl, token, fileName) {
    axios.get(fileUrl, {
        headers: {
            "Authorization": "Bearer " + token,
        },
        responseType: "blob"
    }).then((rs) => {
        const url = window.URL.createObjectURL(rs.data);
        const element = document.createElement("a");
        element.href = url;
        element.download = fileName;
        document.body.appendChild(element);
        element.click();
    });
};

interop.downloadFileFromStream = async (fileName, contentStreamReference) => {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
}

interop.uploadFile = function (url, token, file, dotNetObj) {

    if (!this.selectedFile) {
        return;
    }

    const formData = new FormData();
    formData.append("formFile", this.selectedFile);
    formData.append("name", file.name);
    formData.append("description", file.description);
    formData.append("encrypted", file.encrypted);

    const promise = axios.post(url, formData, {
        headers: {
            "Authorization": "Bearer " + token,
        }
    });

    promise.then((rs) => {
        const id = rs.data.id;
        dotNetObj.invokeMethodAsync("Uploaded", id);
    });
};

interop.fileSelected = function (e) {
    this.selectedFile = e.files ? e.files[0] : null;
};