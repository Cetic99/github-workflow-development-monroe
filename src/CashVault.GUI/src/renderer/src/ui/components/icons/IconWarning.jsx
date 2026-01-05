/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { sizeMap } from './size-map'
import { IconContainer } from './styles'

const IconWarning = ({ color = 'black', size = 's' }) => {
  return (
    <IconContainer color={color} w={sizeMap[size].w} h={sizeMap[size].h}>
      <svg
        width="46"
        height="46"
        viewBox="0 0 46 46"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M35.9997 30.8714L26.4301 11.6985C25.0184 8.86999 20.9823 8.86995 19.5705 11.6985L10.0006 30.8714C8.72851 33.42 10.582 36.4167 13.4304 36.4167H32.5699C35.4183 36.4167 37.2718 33.4201 35.9997 30.8714ZM29.86 9.98662C27.0364 4.32952 18.9643 4.32949 16.1406 9.98657L6.5708 29.1595C4.02658 34.2567 7.73349 40.25 13.4304 40.25H32.5699C38.2668 40.25 41.9737 34.2568 39.4296 29.1595L29.86 9.98662Z"
          fill="currentColor"
        />
        <path
          d="M24.9166 15.3334C24.9166 14.2748 24.0585 13.4167 22.9999 13.4167C21.9414 13.4167 21.0833 14.2748 21.0833 15.3334V23C21.0833 24.0586 21.9414 24.9167 22.9999 24.9167C24.0585 24.9167 24.9166 24.0586 24.9166 23V15.3334Z"
          fill="currentColor"
        />
        <path
          d="M22.9999 28.75C21.9414 28.75 21.0833 29.6081 21.0833 30.6667C21.0833 31.7252 21.9414 32.5834 22.9999 32.5834C24.0585 32.5834 24.9166 31.7252 24.9166 30.6667C24.9166 29.6081 24.0585 28.75 22.9999 28.75Z"
          fill="currentColor"
        />
      </svg>
    </IconContainer>
  )
}

export default IconWarning
