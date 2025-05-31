"use strict";
self["webpackHotUpdatebengineers"]("content_scripts/content-0",{

/***/ "./content/ContentApp.tsx":
/*!********************************!*\
  !*** ./content/ContentApp.tsx ***!
  \********************************/
/***/ ((module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (/* binding */ ContentApp)
/* harmony export */ });
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! react */ "./node_modules/react/index.js");
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(react__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var _firebase_app__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @firebase/app */ "./node_modules/@firebase/app/dist/esm/index.esm2017.js");
/* harmony import */ var _firebase_firestore__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @firebase/firestore */ "./node_modules/@firebase/firestore/dist/index.esm2017.js");
/* provided dependency */ var __react_refresh_utils__ = __webpack_require__(/*! ./node_modules/@pmmmwh/react-refresh-webpack-plugin/lib/runtime/RefreshUtils.js */ "./node_modules/@pmmmwh/react-refresh-webpack-plugin/lib/runtime/RefreshUtils.js");
__webpack_require__.$Refresh$.runtime = __webpack_require__(/*! ./node_modules/react-refresh/runtime.js */ "./node_modules/react-refresh/runtime.js");

function asyncGeneratorStep(gen, resolve, reject, _next, _throw, key, arg) {
    try {
        var info = gen[key](arg);
        var value = info.value;
    } catch (error) {
        reject(error);
        return;
    }
    if (info.done) {
        resolve(value);
    } else {
        Promise.resolve(value).then(_next, _throw);
    }
}
function _async_to_generator(fn) {
    return function() {
        var self = this, args = arguments;
        return new Promise(function(resolve, reject) {
            var gen = fn.apply(self, args);
            function _next(value) {
                asyncGeneratorStep(gen, resolve, reject, _next, _throw, "next", value);
            }
            function _throw(err) {
                asyncGeneratorStep(gen, resolve, reject, _next, _throw, "throw", err);
            }
            _next(undefined);
        });
    };
}
var _s = __webpack_require__.$Refresh$.signature();




const firebaseConfig = {
    apiKey: "AIzaSyDyUws3vjsFtKJrCxS7xNOxLeNqg2df-pI",
    authDomain: "bengineering-hackrift.firebaseapp.com",
    projectId: "bengineering-hackrift",
    storageBucket: "bengineering-hackrift.firebasestorage.app",
    messagingSenderId: "467110689511",
    appId: "1:467110689511:web:b48a00564cb7b5f36e0042"
};
const app = (0,_firebase_app__WEBPACK_IMPORTED_MODULE_1__.initializeApp)(firebaseConfig);
function getPost(postid) {
    return _getPost.apply(this, arguments);
}
function _getPost() {
    _getPost = _async_to_generator(function*(postid) {
        const db = (0,_firebase_firestore__WEBPACK_IMPORTED_MODULE_2__.getFirestore)(app);
        const postsCol = (0,_firebase_firestore__WEBPACK_IMPORTED_MODULE_2__.collection)(db, 'posts');
        const querySnapshot = yield (0,_firebase_firestore__WEBPACK_IMPORTED_MODULE_2__.getDocs)(postsCol);
        const post = querySnapshot.docs.find((doc)=>doc.data().postid === postid);
        return post === null || post === void 0 ? void 0 : post.data();
    });
    return _getPost.apply(this, arguments);
}
function getPostDocByPostId(postid) {
    return _getPostDocByPostId.apply(this, arguments);
}
function _getPostDocByPostId() {
    _getPostDocByPostId = _async_to_generator(function*(postid) {
        const db = (0,_firebase_firestore__WEBPACK_IMPORTED_MODULE_2__.getFirestore)(app);
        const postsCol = (0,_firebase_firestore__WEBPACK_IMPORTED_MODULE_2__.collection)(db, 'posts');
        const querySnapshot = yield (0,_firebase_firestore__WEBPACK_IMPORTED_MODULE_2__.getDocs)(postsCol);
        return querySnapshot.docs.find((doc)=>doc.data().postid === postid);
    });
    return _getPostDocByPostId.apply(this, arguments);
}
function addPost(postid, approvednotes, reliabilitytag, sourcetag, timestamp, topictag, unapprovednotes, content) {
    return _addPost.apply(this, arguments);
}
function _addPost() {
    _addPost = _async_to_generator(function*(postid, approvednotes, reliabilitytag, sourcetag, timestamp, topictag, unapprovednotes, content) {
        const db = (0,_firebase_firestore__WEBPACK_IMPORTED_MODULE_2__.getFirestore)(app);
        const postDoc = yield getPostDocByPostId(postid);
        if (postDoc) {
            // Use updateDoc:
            const docRef = postDoc.ref;
            yield (0,_firebase_firestore__WEBPACK_IMPORTED_MODULE_2__.updateDoc)(docRef, {
                approvednotes: [
                    ...postDoc.data().approvednotes,
                    ...approvednotes
                ],
                reliabilitytag: [
                    ...postDoc.data().reliabilitytag,
                    ...reliabilitytag
                ],
                sourcetag: [
                    ...postDoc.data().sourcetag,
                    ...sourcetag
                ],
                timestamp: timestamp.toISOString(),
                topictag: [
                    ...postDoc.data().topictag,
                    ...topictag
                ],
                unapprovednotes: [
                    ...postDoc.data().unapprovednotes,
                    ...unapprovednotes
                ],
                content: postDoc.data().content
            });
        } else {
            // Use addDoc only when creating a new post:
            const db = (0,_firebase_firestore__WEBPACK_IMPORTED_MODULE_2__.getFirestore)(app);
            const postsCol = (0,_firebase_firestore__WEBPACK_IMPORTED_MODULE_2__.collection)(db, 'posts');
            yield (0,_firebase_firestore__WEBPACK_IMPORTED_MODULE_2__.addDoc)(postsCol, {
                approvednotes,
                postid,
                reliabilitytag,
                sourcetag,
                timestamp: timestamp.toISOString(),
                topictag,
                unapprovednotes,
                content
            });
        }
    });
    return _addPost.apply(this, arguments);
}
// async function addPost(postid: string, approvednotes: string[], reliabilitytag: string[], sourcetag: string[], timestamp: Date, topictag: string[], unapprovednotes: string[], content: string) {
//     const db = getFirestore(app);
//     const postsCol = collection(db, 'posts') as CollectionReference<Post>;
//     console.log("add")
//     console.log(timestamp)
//     console.log(postid)
//     // Update the document if it already exists
//     const post = await getPost(postid);
//     if (post) {
//         await addDoc(postsCol, {
//             approvednotes: [...post.approvednotes, ...approvednotes],
//             postid,
//             reliabilitytag: [...post.reliabilitytag, ...reliabilitytag],
//             sourcetag: [...post.sourcetag, ...sourcetag],
//             timestamp: timestamp.toISOString(),
//             topictag: [...post.topictag, ...topictag],
//             unapprovednotes: [...post.unapprovednotes, ...unapprovednotes],
//             content: post.content,
//         });
//     } else {
//         await addDoc(postsCol, {
//             approvednotes: [...approvednotes],
//             postid,
//             reliabilitytag: [...reliabilitytag],
//             sourcetag: [...sourcetag],
//             timestamp: timestamp.toISOString(),
//             topictag: [...topictag],
//             unapprovednotes: [...unapprovednotes],
//             content: content,
//         });
//     }
// }
function ContentApp() {
    _s();
    (0,react__WEBPACK_IMPORTED_MODULE_0__.useEffect)(()=>{
        const targetNode = document.querySelector('div[role="main"]');
        function processElement(element) {
            return _processElement.apply(this, arguments);
        }
        function _processElement() {
            _processElement = // **Changed**: Define an async function to handle the element processing
            _async_to_generator(function*(element) {
                const today = new Date();
                let timestamp = new Date(today); // Copy current date
                // Your timestamp logic
                const textContent = element.textContent || "";
                if (textContent.includes("hours ago") || textContent.includes("minutes ago")) {
                    timestamp = today;
                } else if (textContent.includes("days ago")) {
                    const match = textContent.match(/^(\d+)\s+days\s+ago$/);
                    if (match) {
                        const days = parseInt(match[1], 10);
                        timestamp.setDate(today.getDate() - days);
                    }
                } else if (textContent.includes("day ago")) {
                    timestamp.setDate(today.getDate() - 1);
                } else if (/(\d{1,2})\s([A-Za-z]+)\s(\d{4})/.test(textContent)) {
                    timestamp = extractDateYear(textContent);
                } else if (/(\d{1,2})\s([A-Za-z]+)/.test(textContent)) {
                    const appendedDate = appendCurrentYear(textContent);
                    if (appendedDate) {
                        timestamp = new Date(appendedDate);
                    }
                } else {
                    timestamp.setDate(today.getDate() + 1);
                }
                let parent = element.parentElement;
                for(let i = 0; i < 8; i++){
                    if (!parent) break;
                    parent = parent.parentElement;
                }
                if (!parent) return;
                let href = parent.querySelectorAll('a');
                var event = new FocusEvent('focusin', {
                    view: window,
                    bubbles: true,
                    cancelable: true
                });
                href.forEach((link)=>link.dispatchEvent(event));
                const filteredHref = Array.from(href).filter((link)=>link.href.includes('/posts'));
                if (filteredHref.length === 0) {
                    console.log('No post link found for this element.');
                    return;
                }
                const filteredHrefStr = filteredHref[0].href;
                console.log(filteredHrefStr);
                const baseHref = filteredHrefStr.split('?')[0];
                if (parent.querySelector('button') === null) {
                    var _parent_querySelector;
                    let content = (_parent_querySelector = parent.querySelector('[data-ad-comet-preview="message"]')) === null || _parent_querySelector === void 0 ? void 0 : _parent_querySelector.textContent;
                    console.log(content);
                    // **Changed**: Now we can await getPost here
                    const postData = yield getPost(baseHref);
                    console.log(postData);
                    // Now proceed with DOM manipulation based on postData
                    const mainContainer = document.createElement('div');
                    mainContainer.classList.add("extension-container");
                    let sourceTag = "None Yet";
                    let topicTags = [
                        "None"
                    ];
                    let reliabilityTag = "None Yet";
                    let communityNote = "None Yet";
                    if (postData) {
                        sourceTag = getMostFrequentTag(postData.sourcetag) || "None Yet";
                        topicTags = postData.topictag || [
                            "None"
                        ];
                        reliabilityTag = getMostFrequentTag(postData.reliabilitytag) || "None Yet";
                        communityNote = postData.approvednotes || "None Yet";
                    } else {
                        sourceTag = "None Yet";
                        topicTags = [
                            "None"
                        ];
                        reliabilityTag = "None Yet";
                        communityNote = "None Yet";
                    }
                    // Create and append the source tag
                    const sourceTagDiv = document.createElement('div');
                    sourceTagDiv.className = 'tag source-tag';
                    sourceTagDiv.textContent = sourceTag; // Assuming sourceTag is a variable holding a string
                    mainContainer.appendChild(sourceTagDiv);
                    // Create and append the reliability tag
                    const reliabilityTagDiv = document.createElement('div');
                    reliabilityTagDiv.className = 'tag reliability-tag';
                    reliabilityTagDiv.textContent = reliabilityTag; // Assuming reliabilityTag is a variable holding a string
                    mainContainer.appendChild(reliabilityTagDiv);
                    // Create and append the show-more button
                    const showMoreBtn = document.createElement('div');
                    showMoreBtn.className = 'show-more-btn';
                    showMoreBtn.textContent = 'V';
                    showMoreBtn.setAttribute('onClick', 'expand()');
                    mainContainer.appendChild(showMoreBtn);
                    // Create and append the extra content container
                    const extraContentDiv = document.createElement('div');
                    extraContentDiv.className = 'hidden extra-content';
                    // Notes paragraph
                    const notesP = document.createElement('p');
                    notesP.className = 'tags-p';
                    notesP.innerHTML = `<span class="title">Notes: </span>${communityNote}`;
                    extraContentDiv.appendChild(notesP);
                    // Relevant Topics paragraph
                    const relevantTopicsP = document.createElement('p');
                    relevantTopicsP.className = 'tags-p';
                    relevantTopicsP.innerHTML = '<span class="title">Relevant Topics: </span><span>tagname</span>'; // Static content
                    extraContentDiv.appendChild(relevantTopicsP);
                    // User Topics paragraph
                    const userTopicsP = document.createElement('p');
                    userTopicsP.className = 'tags-p';
                    var tagsOutput = "";
                    for(var i = 0; i < topicTags.length; i++){
                        tagsOutput += '<span class="user-topic">' + topicTags[i] + "</span>";
                    }
                    userTopicsP.innerHTML = '<span class="title">User Topics: ' + tagsOutput + '</span>'; // Static content
                    extraContentDiv.appendChild(userTopicsP);
                    // Add Tag or Note paragraph
                    const addTagOrNoteP = document.createElement('p');
                    addTagOrNoteP.className = 'tags-p';
                    // Title span
                    const addTagOrNoteTitle = document.createElement('span');
                    addTagOrNoteTitle.className = 'title';
                    addTagOrNoteTitle.textContent = 'Add a Tag or Note: ';
                    addTagOrNoteP.appendChild(addTagOrNoteTitle);
                    // Add Note div
                    const addNoteDiv = document.createElement('div');
                    addNoteDiv.className = 'add-note';
                    addNoteDiv.textContent = 'Add Note';
                    addNoteDiv.setAttribute('onClick', 'openmodal1()');
                    addTagOrNoteP.appendChild(addNoteDiv);
                    // Add Tag div
                    const addTagDiv = document.createElement('div');
                    addTagDiv.className = 'add-tag';
                    addTagDiv.textContent = 'Add Tag';
                    addTagDiv.setAttribute('onClick', 'openmodal2()');
                    addTagOrNoteP.appendChild(addTagDiv);
                    // Append the Add Tag or Note paragraph to extra content
                    extraContentDiv.appendChild(addTagOrNoteP);
                    // Append the extra content to the main container
                    mainContainer.appendChild(extraContentDiv);
                    const addTagModal = document.createElement('div');
                    addTagModal.className = 'add-tag-modal hidden';
                    const addTagForm = document.createElement('form');
                    addTagForm.id = "add-tag-form";
                    addTagForm.innerHTML = '<h3>Add Tags</h3>\
<div class= "form-group">\
<span>Reliability:</span>\
 <input type="radio" id="reliable" name="reliability" value="Reliable">\
<label for="reliable">Reliable</label><br>\
<input type="radio" id="mostly-reliable" name="reliability" value="Mostly-Reliable">\
<label for="mostly-reliable">Mostly-Reliable</label><br>\
<input type="radio" id="mixed" name="reliability" value="Mixed">\
<label for="mixed">Mixed</label>\
<input type="radio" id="likely-false" name="reliability" value="Likely-False">\
<label for="likely-false">Likely-False</label>\
<input type="radio" id="false" name="reliability" value="False">\
<label for="false">False</label></div>\
<div class= "form-group">\
<span>Source:</span>\
 <input type="radio" id="ai" name="source" value="AI">\
<label for="ai">AI</label><br>\
<input type="radio" id="Opinion" name="source" value="Opinion">\
<label for="Opinion">Opinion</label><br>\
 <input type="radio" id="news" name="source" value="News">\
<label for="news">News</label><br>\
 <input type="radio" id="Advertising" name="source" value="Advertising">\
<label for="Advertising">Advertising</label><br>\
<div class= "form-group">\
  <label for="topic"><span>Topic:</span></label>\
  <input type="text" id="topic" name="topic"><br>\
</div>';
                    const addTagButton = document.createElement('button');
                    addTagButton.className = "form-button";
                    addTagButton.addEventListener('click', (event)=>submitTags(event, filteredHrefStr, timestamp, content, addTagForm));
                    addTagButton.innerText = "Add Tags";
                    addTagForm.append(addTagButton);
                    addTagModal.appendChild(addTagForm);
                    const addNoteForm = document.createElement('form');
                    addNoteForm.id = "add-note-form";
                    addNoteForm.innerHTML = '<h3>Add Notes</h3>\
<div class= "form-group">\
  <label for="note"><span>Note:</span></label>\
  <input type="text" id="note" name="note"><br>\
</div>';
                    const addNoteModal = document.createElement('div');
                    addNoteModal.className = 'add-note-modal hidden';
                    const addNoteButton = document.createElement('button');
                    addNoteButton.className = "form-button";
                    addNoteButton.addEventListener('click', (event)=>submitNotes(event, filteredHrefStr, timestamp, content, addNoteForm));
                    addNoteButton.innerText = "Add Notes";
                    addNoteForm.appendChild(addNoteButton);
                    addNoteModal.appendChild(addNoteForm);
                    mainContainer.appendChild(addTagModal);
                    mainContainer.appendChild(addNoteModal);
                    const existingContainer = parent.querySelector('.extension-container');
                    if (existingContainer) {
                        const sourceEl = existingContainer.querySelector('.source-tag');
                        if (sourceEl) sourceEl.textContent = sourceTag;
                        const reliabilityEl = existingContainer.querySelector('.reliability-tag');
                        if (reliabilityEl) reliabilityEl.textContent = reliabilityTag;
                        const titleEl = existingContainer.querySelector('.tags-p .title');
                        if (titleEl && titleEl.nextSibling) {
                            // nextSibling is a Node, which has textContent
                            titleEl.nextSibling.textContent = communityNote;
                        }
                    // Update other fields as needed
                    } else {
                        // Insert the container as you currently do
                        if (parent.querySelector('.extension-container') === null) {
                            parent.insertBefore(mainContainer, parent.children[2]);
                        }
                    }
                }
            });
            return _processElement.apply(this, arguments);
        }
        const observer = new MutationObserver(()=>{
            const elements = document.querySelectorAll('span.x4k7w5x.x1h91t0o.x1h9r5lt.x1jfb8zj.xv2umb2.x1beo9mf.xaigb6o.x12ejxvf.x3igimt.xarpa2k.xedcshv.x1lytzrv.x1t2pt76.x7ja8zs.x1qrby5j');
            const filteredElements = Array.from(elements).filter((element)=>{
                const text = element.textContent || "";
                return /(\d{1,2})\s(January|February|March|April|May|June|July|August|September|October|November|December)/i.test(text) || text.includes('ago') || text.includes('AM') || text.includes('PM');
            });
            // **Changed**: Call processElement for each element without await
            filteredElements.forEach((element)=>{
                processElement(element).catch((err)=>console.error(err));
            });
        });
        if (targetNode) {
            observer.observe(targetNode, {
                attributes: true,
                childList: true,
                subtree: true
            });
        }
        return ()=>observer.disconnect();
    }, []);
    return null;
}
_s(ContentApp, "OD7bBpZva5O2jO+Puf00hKivP7c=");
_c = ContentApp;
function submitTags(e, postid, timestamp, content, form) {
    return _submitTags.apply(this, arguments);
}
function _submitTags() {
    _submitTags = // Function to handle form submission
    _async_to_generator(function*(e, postid, timestamp, content, form) {
        var _form_querySelector, _form_querySelector1, _form_querySelector2;
        console.log("submit");
        // Get the form element
        e.preventDefault();
        // Get the selected radio button value
        // const reliabilitytag = (form?.querySelector('input[name="reliability"]:checked') as HTMLInputElement)?.value;
        // console.log(reliabilitytag)
        // console.log(postid)
        // console.log(timestamp)
        // console.log(content)
        // const sourcetag = (form?.querySelector('input[name="source"]:checked') as HTMLInputElement)?.value;
        // console.log(sourcetag)
        // const topictag = (form?.querySelector('input[name="topic"]') as HTMLInputElement)?.value;
        // console.log(topictag)
        e.preventDefault();
        const reliabilitytag = form === null || form === void 0 ? void 0 : (_form_querySelector = form.querySelector('input[name="reliability"]:checked')) === null || _form_querySelector === void 0 ? void 0 : _form_querySelector.value;
        const sourcetag = form === null || form === void 0 ? void 0 : (_form_querySelector1 = form.querySelector('input[name="source"]:checked')) === null || _form_querySelector1 === void 0 ? void 0 : _form_querySelector1.value;
        const topictag = form === null || form === void 0 ? void 0 : (_form_querySelector2 = form.querySelector('input[name="topic"]')) === null || _form_querySelector2 === void 0 ? void 0 : _form_querySelector2.value;
        const baseHref = postid.split('?')[0];
        if (reliabilitytag) {
            yield addPost(baseHref, [], [
                reliabilitytag
            ], [
                sourcetag
            ], timestamp, [
                topictag
            ], [], content);
        } else {
            alert("Please select a tag before submitting.");
        }
    });
    return _submitTags.apply(this, arguments);
}
function submitNotes(e, postid, timestamp, content, form) {
    return _submitNotes.apply(this, arguments);
}
function _submitNotes() {
    _submitNotes = // Function to handle form submission
    _async_to_generator(function*(e, postid, timestamp, content, form) {
        var _form_querySelector;
        e.preventDefault();
        const notes = form === null || form === void 0 ? void 0 : (_form_querySelector = form.querySelector('input[name="note"]')) === null || _form_querySelector === void 0 ? void 0 : _form_querySelector.value;
        // Normalize postid by removing query parameters
        const baseHref = postid.split('?')[0];
        if (notes) {
            yield addPost(baseHref, [
                notes
            ], [], [], timestamp, [], [], content); // Add to approvednotes
        } else {
            alert("Please enter a note before submitting.");
        }
    });
    return _submitNotes.apply(this, arguments);
}
function extractDateYear(element) {
    const dateRegex = /(\d{1,2})\s(January|February|March|April|May|June|July|August|September|October|November|December)\s(\d{4})/i;
    const match = element.match(dateRegex);
    if (match) {
        const day = parseInt(match[1], 10);
        const month = match[2];
        const year = parseInt(match[3], 10);
        console.log(`Day: ${day}, Month: ${month}, Year: ${year}`);
        // Return a formatted date string (e.g., "2 October 2023")
        return {
            day,
            month,
            year
        };
    } else {
        return new Date(); // No valid date was found
    }
}
function appendCurrentYear(text) {
    // Get the current year
    const currentYear = new Date().getFullYear();
    // Regular expression to match a day and month (e.g., "2 October")
    const dateRegex = /(\d{1,2})\s(January|February|March|April|May|June|July|August|September|October|November|December)/i;
    const match = text.match(dateRegex);
    if (match) {
        const day = match[1];
        const month = match[2];
        // Return the formatted string with the current year appended
        return `${day} ${month} ${currentYear}`;
    }
    return null; // No match found
}
function getMostFrequentTag(tags) {
    if (tags.length === 0) {
        return null; // Return null if the array is empty
    }
    // Define an object to hold counts with string keys and number values
    const tagCounts = {};
    tags.forEach((tag)=>{
        tagCounts[tag] = (tagCounts[tag] || 0) + 1; // Increment count for each tag
    });
    // Find the tag with the highest count
    let mostFrequentTag = null;
    let maxCount = 0;
    for (const [tag, count] of Object.entries(tagCounts)){
        if (count > maxCount) {
            maxCount = count;
            mostFrequentTag = tag;
        }
    }
    return mostFrequentTag;
}
function getPermanentPostLink(tempUrl) {
    return _getPermanentPostLink.apply(this, arguments);
}
function _getPermanentPostLink() {
    _getPermanentPostLink = _async_to_generator(function*(tempUrl) {
        try {
            // Send a request with redirect: 'manual' to prevent automatic redirects
            const response = yield fetch(tempUrl, {
                method: 'GET',
                redirect: 'manual'
            });
            // Check if a redirect is present
            const redirectUrl = response.headers.get('Location');
            if (redirectUrl) {
                console.log('Permanent URL:', redirectUrl);
            } else {
                console.log('No redirect found.');
            }
        } catch (error) {
            console.error('Error fetching the URL:', error);
        }
    });
    return _getPermanentPostLink.apply(this, arguments);
}
// Replace 'TEMP_POST_URL' with the temporary URL you have
const tempPostUrl = 'https://www.facebook.com/some-temporary-url';
getPermanentPostLink(tempPostUrl);
var _c;
__webpack_require__.$Refresh$.register(_c, "ContentApp");


const $ReactRefreshModuleId$ = __webpack_require__.$Refresh$.moduleId;
const $ReactRefreshCurrentExports$ = __react_refresh_utils__.getModuleExports(
	$ReactRefreshModuleId$
);

function $ReactRefreshModuleRuntime$(exports) {
	if (true) {
		let errorOverlay;
		if (true) {
			errorOverlay = false;
		}
		let testMode;
		if (typeof __react_refresh_test__ !== 'undefined') {
			testMode = __react_refresh_test__;
		}
		return __react_refresh_utils__.executeRuntime(
			exports,
			$ReactRefreshModuleId$,
			module.hot,
			errorOverlay,
			testMode
		);
	}
}

if (typeof Promise !== 'undefined' && $ReactRefreshCurrentExports$ instanceof Promise) {
	$ReactRefreshCurrentExports$.then($ReactRefreshModuleRuntime$);
} else {
	$ReactRefreshModuleRuntime$($ReactRefreshCurrentExports$);
}

/***/ })

},
/******/ function(__webpack_require__) { // webpackRuntimeModules
/******/ /* webpack/runtime/getFullHash */
/******/ (() => {
/******/ 	__webpack_require__.h = () => ("b6a0663cd2ad1312d44a")
/******/ })();
/******/ 
/******/ }
);
//# sourceMappingURL=content-0.6d9962136a1398b46d90.js.map