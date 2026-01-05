/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { sizeMap } from './size-map'
import { IconContainer } from './styles'

const IconClose = ({ color = 'black', size = 's' }) => {
  return (
    <IconContainer color={color} w={sizeMap[size].w} h={sizeMap[size].h}>
      <svg
        width="24"
        height="24"
        viewBox="0 0 24 24"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          d="M17.6569 6.3427C17.2664 5.95217 16.6332 5.95217 16.2427 6.3427L11.9997 10.5857L7.7574 6.34344C7.36688 5.95292 6.73371 5.95292 6.34319 6.34344C5.95266 6.73397 5.95266 7.36713 6.34319 7.75766L10.5855 11.9999L6.34321 16.2422C5.95269 16.6327 5.95269 17.2659 6.34321 17.6564C6.73374 18.0469 7.3669 18.0469 7.75743 17.6564L11.9997 13.4142L16.2427 17.6572C16.6332 18.0477 17.2664 18.0477 17.6569 17.6572C18.0474 17.2666 18.0474 16.6335 17.6569 16.2429L13.4139 11.9999L17.6569 7.75691C18.0474 7.36639 18.0474 6.73322 17.6569 6.3427Z"
          fill="currentColor"
        />
      </svg>
    </IconContainer>
  )
}

export default IconClose
