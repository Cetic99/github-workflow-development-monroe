/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { sizeMap } from './size-map'
import { IconContainer } from './styles'

const IconKeyboardSettings = ({ color = 'black', size = 's' }) => {
  return (
    <IconContainer color={color} w={sizeMap[size].w} h={sizeMap[size].h}>
      <svg
        width="48"
        height="48"
        viewBox="0 0 48 48"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          d="M8 10C8 8.89543 8.89543 8 10 8H14C15.1046 8 16 8.89543 16 10V14C16 15.1046 15.1046 16 14 16H10C8.89543 16 8 15.1046 8 14V10Z"
          fill="currentColor"
        />
        <path
          d="M8 22C8 20.8954 8.89543 20 10 20H14C15.1046 20 16 20.8954 16 22V26C16 27.1046 15.1046 28 14 28H10C8.89543 28 8 27.1046 8 26V22Z"
          fill="currentColor"
        />
        <path
          d="M22 8C20.8954 8 20 8.89543 20 10V14C20 15.1046 20.8954 16 22 16H26C27.1046 16 28 15.1046 28 14V10C28 8.89543 27.1046 8 26 8H22Z"
          fill="currentColor"
        />
        <path
          d="M20 22C20 20.8954 20.8954 20 22 20H26C27.1046 20 28 20.8954 28 22V26C28 27.1046 27.1046 28 26 28H22C20.8954 28 20 27.1046 20 26V22Z"
          fill="currentColor"
        />
        <path
          d="M22 32C20.8954 32 20 32.8954 20 34V38C20 39.1046 20.8954 40 22 40H26C27.1046 40 28 39.1046 28 38V34C28 32.8954 27.1046 32 26 32H22Z"
          fill="currentColor"
        />
        <path
          d="M32 10C32 8.89543 32.8954 8 34 8H38C39.1046 8 40 8.89543 40 10V14C40 15.1046 39.1046 16 38 16H34C32.8954 16 32 15.1046 32 14V10Z"
          fill="currentColor"
        />
        <path
          d="M34 20C32.8954 20 32 20.8954 32 22V26C32 27.1046 32.8954 28 34 28H38C39.1046 28 40 27.1046 40 26V22C40 20.8954 39.1046 20 38 20H34Z"
          fill="currentColor"
        />
      </svg>
    </IconContainer>
  )
}

export default IconKeyboardSettings
